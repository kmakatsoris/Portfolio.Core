using System.Diagnostics;
using ILogger = NLog.ILogger;

namespace Portfolio.Core.CleanAndBackup.Worker.Utils {
    public static class DefaultUtils {
        public static int ExecuteBashScript(this ILogger _logger, string scriptPath)
        {
            if (string.IsNullOrEmpty(scriptPath)) return -1;
            var processInfo = new ProcessStartInfo
            {
                FileName = "bash",
                Arguments = scriptPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                if (process == null) {
                    _logger.Error("Failed to start process.");
                    return -1;
                }

                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                _logger.Error($"Output: {output}\nError: {error}");
                return !string.IsNullOrEmpty(error) ? 0 : -2;                
            }
        }
    }
}
