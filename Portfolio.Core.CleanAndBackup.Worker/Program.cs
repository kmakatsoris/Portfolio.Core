using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using Portfolio.Core.CleanAndBackup.Worker;
using Portfolio.Core.CleanAndBackup.Worker.Types;

namespace Portfolio.Core.CleanAndBackup.Worker {
    public static class Program
    {
        public static void Main(string[] args)
        {
            // Initialize NLog
            var logger = LogManager.Setup()
                .LoadConfigurationFromAppSettings()
                .GetCurrentClassLogger();

            try
            {
                logger.Warn("Starting up the service...");

                var builder = Host.CreateApplicationBuilder(args);

                // Configure IConfigurationService
                IConfigurationRoot config;
                builder.Services.ConfigureIConfigurationService(out config);

                // Register NLog with the dependency injection container    
                builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                builder.Logging.AddNLog();

                // Register the worker service
                builder.Services.AddHostedService<Worker>();

                var host = builder.Build();

                host.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while starting the service.");
            }
            finally
            {
                LogManager.Shutdown(); // Ensure NLog flushes and closes properly
            }
        }

        // Extension method to configure IConfigurationService
        private static void ConfigureIConfigurationService(this IServiceCollection services, out IConfigurationRoot config)
        {
            // Load configuration based on the environment
                var configuration = new ConfigurationBuilder()
                    // .SetBasePath(hostContext.HostingEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
                    .Build();

                config = configuration;

                // Bind configuration to AppSettings class and add it to the service collection                
                services.Configure<AppSettings>(settings =>
                {
                    settings.PreventStorageCollapseWorkerConfig = configuration.GetSection("PreventStorageCollapseWorkerConfig").Get<PreventStorageCollapseWorkerConfig>();
                    settings.BackupWorkerConfig = configuration.GetSection("BackupWorkerConfig").Get<BackupWorkerConfig>();
                    settings.CleanupWorkerConfig = configuration.GetSection("CleanupWorkerConfig").Get<CleanupWorkerConfig>();
                });
        }
    }
}