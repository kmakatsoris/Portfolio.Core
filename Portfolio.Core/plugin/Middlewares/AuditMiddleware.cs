using System.Diagnostics;
using Newtonsoft.Json;
using NLog;
using Portfolio.Core.Exceptions;
using Portfolio.Core.Performance;
using Portfolio.Core.Types.PlugIns.Types.AuditMiddleware;
using ILogger = NLog.ILogger;

namespace Portfolio.Core.Middlewares
{
    public class AuditMiddleware_InvokeAsyncResponse
    {
        public string logMessage { get; set; }
        public int status  { get; set; }
    }
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;        
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<string, string[]> _excludedEndpoints;
        private readonly string[] _filteredEndpoints;
        private AuditMiddleware_InvokeAsyncResponse invokeRes;   

        public AuditMiddleware(RequestDelegate next, AuditMiddlewareOptions options)
        {
            _next = next;
            _excludedEndpoints = options?.ExcludedEndpoints ?? new Dictionary<string, string[]>();  
            _filteredEndpoints = options?.FilteredEndpoint ?? [];         
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {    
                string logMessage = "";                
                string performanceDiagnostics = await PerformanceMetrics.PerformanceMetricsAsync(async () =>
                {
                    await InvokeAsyncHelper(context, logMessage);
                });
                if (!string.IsNullOrEmpty(invokeRes?.logMessage)) invokeRes.logMessage += "[Performance Diagnostics]:"+performanceDiagnostics+"\n\n";
                DefaultException.Throw(null, invokeRes?.logMessage ?? "", new DefaultExceptionConfig() {
                    exLevel = DefaultException.GetLogLevel(invokeRes?.status ?? -1)
                });                               
            }
            catch (Exception ex)
            {
                _logger.Error($"Internal Error Happened Inside The AuditMiddleware\n\nException: {ex?.Message}");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(ex?.Message ?? "Internal Error Happened");
            }
        }

        private async Task InvokeAsyncHelper(HttpContext context, string logMessage)
        {
            string filteredTxt = "***********";
            context.Request.EnableBuffering();
            var requestBody = await ReadRequestBodyAsync(context.Request);
            
            var originalResponseBody = context?.Response?.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                context.Response.Body = originalResponseBody;
                responseBody.Seek(0, SeekOrigin.Begin);
                var responseBodyText = await new StreamReader(responseBody).ReadToEndAsync();
                responseBody.Seek(0, SeekOrigin.Begin);

                var statusCode = context?.Response?.StatusCode;
                var endpointName = context.Request.Path.Value.Trim('/');

                if (!_excludedEndpoints.ContainsKey(endpointName) || 
                    !Array.Exists(_excludedEndpoints[endpointName], code => string.Equals(code, statusCode.ToString(), StringComparison.OrdinalIgnoreCase)) ||
                    DefaultException.enThrowException
                   )
                {                    
                    bool isNOT_FilteredEndpoint = !_filteredEndpoints.Any(e => string.Equals(e, endpointName, StringComparison.OrdinalIgnoreCase));
                    string requestHeader = isNOT_FilteredEndpoint ? JsonConvert.SerializeObject(context.Request.Headers) : filteredTxt;
                    string requestPayload = isNOT_FilteredEndpoint ? requestBody : filteredTxt;
                    string responseHeader = isNOT_FilteredEndpoint ? JsonConvert.SerializeObject(context.Response.Headers) : filteredTxt;
                    string responsePayload = isNOT_FilteredEndpoint ? responseBodyText : filteredTxt;

                    logMessage += $"Id: {Guid.NewGuid()}\n" +                                        
                                    $"BaseUrl: {context.Request.Scheme}://{context.Request.Host}\n" +
                                    $"Port: {context.Request.Host.Port}\n" +
                                    $"Endpoint: {endpointName}\n" +
                                    $"Status: {statusCode}\n" +
                                    $"RequestHeader: {requestHeader}\n" +
                                    $"RequestPayload: {requestPayload}\n" +
                                    $"ResponseHeader: {responseHeader}\n" +
                                    $"ResponsePayload: {responsePayload}\n" +
                                    $"Date&Time: {DateTime.UtcNow}\n";                                          
                }

                await responseBody.CopyToAsync(originalResponseBody);
                invokeRes = new AuditMiddleware_InvokeAsyncResponse {
                                logMessage = logMessage,
                                status = statusCode ?? -1
                            };                
            }
        }

        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.EnableBuffering();
            using (var reader = new StreamReader(request.Body, leaveOpen: true))
            {
                var body = await reader.ReadToEndAsync();
                request.Body.Position = 0;  // Reset the stream position for subsequent middleware
                return body;
            }
        }
    }
}
