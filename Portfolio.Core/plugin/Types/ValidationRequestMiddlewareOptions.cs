using ILogger = NLog.ILogger;

namespace Portfolio.Core.Types.PlugIns.Types.JwtValidationMiddlewareOptions
{
    public class ValidationRequestMiddlewareOptions
    {
        public RequestDelegate Next { get; set; }
        public string[] ExcludedEndpoints { get; set; }
        public ILogger Logger { get; set; }
    }
}