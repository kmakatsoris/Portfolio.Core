namespace Portfolio.Core.Types.PlugIns.Types.AuditMiddleware
{
    public class AuditMiddlewareOptions
    {
        public RequestDelegate Next { get; set; }
        public Dictionary<string, string[]> ExcludedEndpoints { get; set; }              
        public string[] FilteredEndpoint { get; set; }       
    }
}