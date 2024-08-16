using Microsoft.Extensions.Primitives;
using Portfolio.Core.Interfaces.Identity;
using Portfolio.Core.Types.PlugIns.Types.JwtValidationMiddlewareOptions;

namespace Portfolio.Core.Middlewares
{
    public class JwtValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOAuthAuthorizationService _oauthService;
        private readonly string[] _excludedEndpoints;

        public JwtValidationMiddleware(RequestDelegate next, JwtValidationMiddlewareOptions options)
        {
            _next = next;
            _excludedEndpoints = options.ExcludedEndpoints;
            _oauthService = options.OAuthAuthorizationService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the requested endpoint is in the exclusion list
            var endpoint = context?.Request?.Path;
            if (Array.Exists(_excludedEndpoints, e => e.Equals(endpoint, StringComparison.OrdinalIgnoreCase)))
            {
                // Endpoint is excluded, proceed to the next middleware
                await _next(context);
                return;
            }

            // Endpoint requires JWT validation, perform validation
            string token = await ExtractToken(context);

            var email = _oauthService?.GetEmailFromToken(token); // Implement this method to extract email from token

            if (await _oauthService?.ValidateUserJwtToken(email, token))
            {
                // Token is valid, proceed to the next middleware
                await _next(context);
            }
            else
            {
                // Token validation failed, return unauthorized response
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden");
            }
        }

        private async Task<string> ExtractToken(HttpContext context)
        {
            StringValues authorizationValue = new StringValues();
            string[] tokens = context?.Request?.Headers.TryGetValue("Authorization", out authorizationValue) == true && !StringValues.IsNullOrEmpty(authorizationValue) ? authorizationValue.FirstOrDefault().ToString().Split(" ") : [];

            string token = "";
            if (tokens != null && tokens?.Length >= 1)
            {
                if (tokens?.Length == 1)
                {
                    token = tokens[0];
                }
                else
                {
                    if (tokens.Contains("Bearer"))
                    {
                        token = tokens[1];
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync("Forbidden");
                        return "";
                    }
                }
            }

            return token;
        }
    }
}