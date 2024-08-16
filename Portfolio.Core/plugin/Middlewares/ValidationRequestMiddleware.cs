using Newtonsoft.Json;
using Portfolio.Core.Types.DTOs.Requests.Identity;
using Portfolio.Core.Types.PlugIns.Types.JwtValidationMiddlewareOptions;
using ILogger = NLog.ILogger;

namespace Portfolio.Core.Middlewares
{
    public class ValidationRequestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly string[] _excludedEndpoints;

        public ValidationRequestMiddleware(RequestDelegate next, ValidationRequestMiddlewareOptions options)
        {
            _next = next;
            _excludedEndpoints = options.ExcludedEndpoints;
            _logger = options.Logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Check if the requested endpoint is in the exclusion list
                var endpoint = context?.Request?.Path;
                if (Array.Exists(_excludedEndpoints, e => e.Equals(endpoint, StringComparison.OrdinalIgnoreCase)))
                {
                    // Endpoint is excluded, proceed to the next middleware
                    await _next(context);
                    return;
                }

                if (context.Request.ContentType != null && context.Request.ContentType.StartsWith("application/json"))
                {
                    context.Request.EnableBuffering();
                    using (var reader = new StreamReader(context.Request.Body))
                    {
                        var requestBodyJson = await reader.ReadToEndAsync();
                        context.Request.Body.Position = 0;  // Reset the stream position for subsequent middleware

                        var requestBody = JsonConvert.DeserializeObject<IdentityBaseRequest>(requestBodyJson);

                        if (requestBody != null && requestBody.Password != null && requestBody.ConfirmPassword != null)
                        {
                            if (requestBody.Password == requestBody.ConfirmPassword)
                            {
                                await _next(context);
                                return;
                            }
                            else
                            {
                                _logger.Error("Validation Error. Password and confirmPassword do not match.");
                                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                await context.Response.WriteAsync("Validation Error. Password and confirmPassword do not match.");
                                return;
                            }
                        }
                    }
                    _logger.Error("Validation Error. Request must include password and confirmPassword.");
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Missing password or confirmPassword.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Internal Error Happened Inside The ValidationRequestMiddleware\n\nException: {ex.Message}");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(ex?.Message ?? "Internal Error Happened");
            }
        }
    }
}
