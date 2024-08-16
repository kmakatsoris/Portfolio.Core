using Portfolio.Core.Interfaces.Identity;

namespace Portfolio.Core.Types.PlugIns.Types.JwtValidationMiddlewareOptions
{
    public class JwtValidationMiddlewareOptions
    {
        public RequestDelegate Next { get; set; }
        public string[] ExcludedEndpoints { get; set; }
        public IOAuthAuthorizationService OAuthAuthorizationService { get; set; }
    }
}