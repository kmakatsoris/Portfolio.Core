using System.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using Portfolio.Core.Implementation.Identity;
using Portfolio.Core.Interfaces.Context.Users;
using Portfolio.Core.Interfaces.Identity;
using Portfolio.Core.Types.Context;
using ILogger = NLog.ILogger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Portfolio.Core.Middlewares;
using Portfolio.Core.Types.PlugIns.Types.JwtValidationMiddlewareOptions;
using System.Text.Json.Serialization;
using Portfolio.Core.Services.Context;
using Portfolio.Core.Interfaces.Reviews;
using Portfolio.Core.Implementation.Reviews;
using Portfolio.Core.Interfaces.Context.Reviews;
using Portfolio.Core.Interfaces.Context.Skills;
using Portfolio.Core.Interfaces.Skills;
using Portfolio.Core.Implementation.Skills;
using Portfolio.Core.Interfaces.Resources;
using Portfolio.Core.Implementation.Resources;
using Portfolio.Core.Interfaces.Context.Resources;
using Portfolio.Core.Interfaces.Context.Projects;
using Portfolio.Core.Interfaces.Projects;
using Portfolio.Core.Implementation.Projects;
using Portfolio.Core.Interfaces.Context.MetaData;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Portfolio.Core.CMS.Strapi;
using Portfolio.Core.Utils.DefaultUtils;
using System.Net.Http.Headers;
using NLog.Extensions.Logging;
using Portfolio.Core.Types.PlugIns.Types.AuditMiddleware;

namespace Portfolio.Core
{
    public static class Program
    {
        private static readonly int DEFAULT_TIMEOUT = 100;

        #region Main Functionality
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            try
            {
                logger.Debug("Welcome Aboard!");
                return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .ConfigureServices((hostContext, services) =>
                    {
                        // Register Basic-Default Services:                        
                        hostContext.ConfigureIConfigurationService(services, out var config);

                        // Add JWT Authentication
                        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options =>
                        {
                            var t = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtConfig:IssuerSigningKey"])), // your_very_long_secret_key_here_to_ensure_security
                                ValidIssuer = config["JwtConfig:ValidIssuer"], // The Issuer claim identifies the principal that issued the JWT. In the context of a web service, this is typically the URL of the service or any identifier that reliably represents the issuer of the token.
                                ValidAudience = config["JwtConfig:ValidAudience"] // The Audience claim identifies the recipients that the JWT is intended for. It should match the identifier of the intended recipient(s) of the JWT to prevent tokens from being accidentally or maliciously used by unintended parties.
                            };
                            options.TokenValidationParameters = t;
                        });

                        // Add Authorization Policies
                        services.AddAuthorization(options =>
                        {
                            // @TAG: DB_ENTITIES
                            options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                            options.AddPolicy("User", policy => policy.RequireRole("User"));
                            options.AddPolicy("AdminOrUser", policy => policy.RequireRole("Admin", "User"));
                            // options.AddPolicy("Client", policy => policy.RequireRole("user").RequireClaim("AccessLevel", "ReadWrite")); // User With Read Write Access
                        });

                        // Register Databases Service:
                        // services.ConfigureDatabasesService(config); -> Is not the right way to open/close the DB Connections

                        // CORS,
                        services.AddCors(options =>
                        {
                            options.AddPolicy("AllowSpecificOrigin",
                                builder => builder
                                    .WithOrigins("http://localhost:3000", "http://localhost")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowCredentials());
                        }); // WithOrigins("http://localhost:3000", "http://localhost:4200")

                        // Register Controllers Middleware:
                        services.AddControllers().AddJsonOptions(opts =>
                        {
                            opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                        });

                        // Register health checks
                        services.AddHealthChecks()
                                .AddMySql(
                                    connectionString: config["ConnectionStrings:MySqlConnection"],
                                    name: "Users-Database:",
                                    failureStatus: HealthStatus.Unhealthy,
                                    tags: new[] { "db", "mysql", "database", "user" })
                                .AddMySql(
                                    connectionString: config["ConnectionStrings:MySqlConnection"],
                                    name: "Other-Database:",
                                    failureStatus: HealthStatus.Unhealthy,
                                    tags: new[] { "db", "mysql", "database", "other function" }); // http://localhost:@port/health

                        // Register Http Clients,
                        services.RegisterHttpClients(config);

                        // Register your services and interfaces here                        
                        services.RegisterServices(logger);

                        // Register the Swagger generator
                        services.AddSwaggerGen(c =>
                        {
                            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Portfolio.Core", Version = "v1" });
                        });
                    })
                    .ConfigureLogging(logging =>
                    {
                        // logging.ClearProviders(); -> Not letting Running the App
                        logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                        logging.AddNLog(); // Make sure NLog is correctly set up
                    })
                    .Configure(app =>
                    {
                        app.UseHttpsRedirection();

                        app.UseRouting();

                        // Enable middleware to serve generated Swagger as a JSON endpoint.
                        app.UseSwagger();

                        // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                        // specifying the Swagger JSON endpoint.
                        app.UseSwaggerUI(c =>
                        {
                            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Portfolio.Core.V1");
                        });

                        // Retrieve the instance of IOAuthAuthorizationService from the DI container
                        var serviceProvider = app.ApplicationServices;

                        // Register Middleware,
                        app.RegisterMiddlewares(serviceProvider);

                        // Enable CORS policy
                        app.UseCors("AllowSpecificOrigin");

                        // Use authentication and authorization
                        app.UseAuthentication();
                        app.UseAuthorization();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();

                            endpoints.MapHealthChecks("/health", new HealthCheckOptions
                            {
                                ResultStatusCodes = {
                                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                        },
                                ResponseWriter = async (context, report) =>
                                {
                                    context.Response.ContentType = "application/json";
                                    var result = JsonSerializer.Serialize(new
                                    {
                                        status = report.Status.ToString(),
                                        checks = report.Entries.Select(x => new
                                        {
                                            name = x.Key,
                                            status = x.Value.Status.ToString(),
                                            exception = x.Value.Exception != null ? x.Value.Exception.Message : "none",
                                            duration = x.Value.Duration.ToString()
                                        })
                                    });
                                    await context.Response.WriteAsync(result);
                                }
                            });
                        });
                    })
                    .UseUrls("http://*:5123");
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex?.Message, "Stopped program because of exception");
                throw new Exception("Program" + ex?.Message);
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }
        #endregion

        #region Configuration Settings Methods
        private static void ConfigureIConfigurationService(this WebHostBuilderContext hostContext, IServiceCollection services, out IConfigurationRoot config)
        {
            if (hostContext.HostingEnvironment.IsDevelopment() || hostContext.HostingEnvironment.IsProduction())
            {
                // Load configuration based on the environment
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(hostContext.HostingEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                configuration = new ConfigurationBuilder()
                    .SetBasePath(hostContext.HostingEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development"}.json", optional: true, reloadOnChange: true)
                    .Build();

                config = configuration;

                // Bind configuration to AppSettings class and add it to the service collection                
                services.Configure<AppSettings>(settings =>
                {
                    settings.AllowedHosts = configuration.ConfigureBasicSettings();
                    settings.JwtConfig = configuration.ConfigureJwtConfig();
                    settings.ConnectionStrings = configuration.ConfigureConnectionStrings();
                    settings.CMS_Strapi_RenderPathsAndCollections = configuration.ConfigureCMS_Strapi_RenderPathsAndCollections();
                });
            }
            else
            {
                throw new Exception("Error on initializing the Configuration Service");
            }
        }

        private static string ConfigureBasicSettings(this IConfiguration configuration)
        {
            return configuration["AllowedHosts"] ?? "";
        }

        private static ConnectionStrings ConfigureConnectionStrings(this IConfiguration configuration)
        {
            return new ConnectionStrings
            {
                IdentityConnection = configuration["ConnectionStrings:MySqlConnection"] ?? "",
                StrapiConnection = configuration["ConnectionStrings:StrapiConnection"] ?? ""
            };
        }

        private static CMS_Strapi_RenderPathsAndCollections ConfigureCMS_Strapi_RenderPathsAndCollections(this IConfiguration configuration)
        {
            return new CMS_Strapi_RenderPathsAndCollections
            {
                HomePage = configuration["CMS_Strapi_RenderPathsAndCollections:HomePage"] ?? "",
                BioPage = configuration["CMS_Strapi_RenderPathsAndCollections:BioPage"] ?? "",
                SkillsPage = configuration["CMS_Strapi_RenderPathsAndCollections:SkillsPage"] ?? "",
                ProjectsPage = configuration["CMS_Strapi_RenderPathsAndCollections:ProjectsPage"] ?? "",
                APIsPage = configuration["CMS_Strapi_RenderPathsAndCollections:APIsPage"] ?? "",
                ContactsPage = configuration["CMS_Strapi_RenderPathsAndCollections:ContactsPage"] ?? "",
            };
        }

        private static JwtConfig ConfigureJwtConfig(this IConfiguration configuration)
        {
            return new JwtConfig
            {
                IssuerSigningKey = configuration["JwtConfig:IssuerSigningKey"] ?? "",
                ValidAudience = configuration["JwtConfig:ValidAudience"] ?? "",
                ValidIssuer = configuration["JwtConfig:ValidIssuer"] ?? "",
                JwtTime = configuration["JwtConfig:JwtTime"] ?? ""
            };
        }

        #endregion

        #region Services Injection
        private static void RegisterHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();
            string[] strapiConfig = configuration["ConnectionStrings:StrapiConnection"]?.Split(";");
            services.AddHttpClient("StrapiClient", client =>
            {
                client.BaseAddress = new Uri(strapiConfig?.GetValue<string>(0));
                client.Timeout = TimeSpan.FromSeconds(int.TryParse(strapiConfig?.GetValue<string>(1), out int t) ? t : DEFAULT_TIMEOUT);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", strapiConfig?.GetValue<string>(2));
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
        }

        private static void RegisterServices(this IServiceCollection services, Logger logger)
        {
            // Singleton Lifecycle:
            // --------------------------------------------------------------------
            services.AddSingleton<ILogger, Logger>(x => logger);
            services.AddSingleton<IOAuthAuthorizationService, OAuthJWTAuthorizationService>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IReviewRepository, ReviewRepository>();
            services.AddSingleton<ISkillsRepository, SkillsRepository>();
            services.AddSingleton<IResourceRepository, ResourcesRepository>();
            services.AddSingleton<IProjectsRepository, ProjectsRepository>();
            services.AddSingleton<IMetaDataRepository, MetaDataRepository>();

            // Scoped Lifecycle:
            // --------------------------------------------------------------------
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IStrapiService, StrapiService>();

            // Transient Lifecycle:
            // --------------------------------------------------------------------
            services.AddTransient<IReviewsService, ReviewsService>();
            services.AddTransient<ISkillsService, SkillsService>();
            services.AddTransient<IResourcesService, ResourcesService>();
            services.AddTransient<IProjectsService, ProjectsService>();
        }
        #endregion

        #region Register Middlewares,
        private static void RegisterMiddlewares(this IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            // Request Validation Middleware,
            app.UseMiddleware<ValidationRequestMiddleware>(new ValidationRequestMiddlewareOptions
            {
                ExcludedEndpoints = new string[]
                            {
                                "/api/Authentication/logout",

                                "/api/Reviews/list",
                                "/api/Reviews/find",
                                "/api/Reviews/insert",
                                "/api/Reviews/edit",
                                "/api/Reviews/delete",

                                "/api/Skills/getAllSkills",
                                "/api/Skills/getSkillsByCategory",
                                "/api/Skills/getSkillsByTags",
                                "/api/Skills/getSkillsCommentedByEmail",
                                "/api/Skills/getAllSkillsByTagsAsync",
                                "/api/Skills/insertSkill",
                                "/api/Skills/updateSkill",
                                "/api/Skills/updateSkillComment",
                                "/api/Skills/deleteSkill",
                                "/api/Skills/deleteSkillComment",

                                "/api/Projects/getAllProjects",
                                "/api/Projects/getProjectsByCategory",
                                "/api/Projects/getProjectsByTags",
                                "/api/Projects/getProjectsCommentedByEmail",
                                "/api/Projects/getAllProjectsByTagsAsync",
                                "/api/Projects/insertProject",
                                "/api/Projects/updateProject",
                                "/api/Projects/updateProjectComment",
                                "/api/Projects/deleteProject",
                                "/api/Projects/deleteProjectComment",

                                "/api/Resources/list",
                                "/api/Resources/find",
                                "/api/Resources/getAllResourcesByTagsAsync",
                                "/api/Resources/insert",
                                "/api/Resources/edit",
                                "/api/Resources/delete",

                                "/api/Strapi/list",
                                "/api/Strapi/find",
                                "/api/Strapi/sync",
                            },
                Logger = serviceProvider.GetService<ILogger>()
            });

            // JWT Middleware,
            app.UseMiddleware<JwtValidationMiddleware>(new JwtValidationMiddlewareOptions
            {
                ExcludedEndpoints = new string[]
                            {
                                "/api/Authentication/login",
                                "/api/Authentication/register",
                                "/api/Authentication/logout",
                                "/api/Authentication/forgotPassword",

                                "/api/Reviews/list",
                                "/api/Reviews/find",

                                "/api/Skills/getAllSkillsByTagsAsync",
                                "/api/Skills/getAllSkills",

                                "/api/Projects/getAllProjectsByTagsAsync",
                                "/api/Projects/getAllProjects",

                                "/api/Resources/getAllResourcesByTagsAsync",
                                "/api/Resources/list",
                                "/api/Resources/find",

                                "/api/Strapi/list",
                                "/api/Strapi/find",
                                "/api/Strapi/sync",
                            },
                OAuthAuthorizationService = serviceProvider.GetService<IOAuthAuthorizationService>()
            });

            // Audit Middleware,
            app.UseMiddleware<AuditMiddleware>(new AuditMiddlewareOptions
            {
                ExcludedEndpoints = new Dictionary<string, string[]>
                {
                    { "api/Authentication/register", new[] { "200" } },
                    { "api/Authentication/registerAdmin", new[] { "200" } },
                    { "api/Authentication/login", new[] { "200" } },
                    { "api/Authentication/logout", new[] { "200" } },
                    { "api/Authentication/forgotPassword", new[] { "200" } },
                    { "api/Authentication/forgotPasswordAdmin", new[] { "200" } },

                    { "api/Projects/getAllProjects", new[] { "200" } },
                    { "api/Projects/getProjectsByCategory", new[] { "200" } },
                    { "api/Projects/getProjectsByTags", new[] { "200" } },
                    { "api/Projects/getAllProjectsByTagsAsync", new[] { "200" } },
                    { "api/Projects/getProjectsCommentedByEmail", new[] { "200" } },

                    { "api/Reviews/list", new[] { "200" } },
                    { "api/Reviews/find", new[] { "200" } },

                    { "api/Skills/getAllSkills", new[] { "200" } },
                    { "api/Skills/getSkillsByCategory", new[] { "200" } },
                    { "api/Skills/getSkillsByTags", new[] { "200" } },
                    { "api/Skills/getAllSkillsByTagsAsync", new[] { "200" } },
                    { "api/Skills/getSkillsCommentedByEmail", new[] { "200" } },

                    { "api/Resources/list", new[] { "200" } },
                    { "api/Resources/find", new[] { "200" } },
                    { "api/Resources/getAllResourcesByTagsAsync", new[] { "200" } },

                    { "api/Strapi/list", new[] { "200" } },
                    { "api/Strapi/find", new[] { "200" } },
                },
                FilteredEndpoint = [
                                    "api/Authentication/register",
                                    "api/Authentication/registerAdmin",
                                    "api/Authentication/login",
                                    "api/Authentication/logout",
                                    "api/Authentication/forgotPassword",
                                    "api/Authentication/forgotPasswordAdmin"
                                   ]
            });
        }
        #endregion

        /*
        private static void ConfigureDatabasesService(this IServiceCollection services, IConfigurationRoot config)
        {
            services.AddScoped<IDbConnection>((sp) => new MySqlConnection(config["ConnectionStrings:MySqlConnection"]));
        }
        */

    }
}