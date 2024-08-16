using System.Diagnostics;
using NLog;
using Portfolio.Core.Exceptions;
using ILogger = NLog.ILogger;

namespace Portfolio.Core.Performance
{
    public class PerformanceMetricsConfig
    {
        public string separatorCh { get; set; }
        public string value { get; set; }
    }
    public static class PerformanceMetrics
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        public static async Task<string> PerformanceMetricsAsync(this Func<Task> action)
        {
            string diagMsg = "";            

            var stopwatch = Stopwatch.StartNew();
            var process = Process.GetCurrentProcess();

            // Capture initial memory and CPU usage
            var initialMemoryUsage = process.PrivateMemorySize64;
            var initialCpuUsage = GetCpuUsage(process);

            // Execute the action
            await action();            

            // Capture final memory and CPU usage
            var finalMemoryUsage = process.PrivateMemorySize64;
            var finalCpuUsage = GetCpuUsage(process);

            stopwatch.Stop();

            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            var memoryUsage = finalMemoryUsage - initialMemoryUsage;
            var cpuUsage = finalCpuUsage - initialCpuUsage;

            diagMsg += $"Execution Time: {elapsedMilliseconds} ms";
            diagMsg += $"Memory Usage: {memoryUsage / 1024.0 / 1024.0} MB";
            diagMsg += $"CPU Usage: {cpuUsage} CPU cycles";

            return diagMsg;                         
        }

        private static long GetCpuUsage(Process process)
        {            
            return process.TotalProcessorTime.Ticks;
        }
    }
}