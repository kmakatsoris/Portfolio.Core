using System;
using System.IO;
using Microsoft.Extensions.Options;
using NLog;
using Portfolio.Core.CleanAndBackup.Worker.Types;
using Portfolio.Core.CleanAndBackup.Worker.Utils;
using ILogger = NLog.ILogger;

namespace Portfolio.Core.CleanAndBackup.Worker.Workers
{
    public class PreventStorageCollapseWorker
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IOptions<AppSettings> _appSettings;
        private readonly Guid _PID;

        public PreventStorageCollapseWorker(IOptions<AppSettings> appSettings, Guid pid)
        {
            _appSettings = appSettings;
            _PID = pid;
        }
        public void Run()
        {
            try
            {
                DiagnosticsType diags = LoadDiagnostics();
                _logger.Warn(diags?.ToString(), DateTimeOffset.Now);
                if (diags?.AvailableSize <= _appSettings?.Value?.PreventStorageCollapseWorkerConfig?.MinAvailableSizeGB)
                    DefaultUtils.ExecuteBashScript(_logger, _appSettings?.Value?.PreventStorageCollapseWorkerConfig?.MinAvailableSizeGBBashScriptPath);
            }
            catch (Exception ex)
            {
                _logger.Error($"@Important: Error executing the Run Command (PID:{_PID}) with exception message: {ex?.Message}");
            }
        }

        #region Private Methods
        private DiagnosticsType LoadDiagnostics()
        {
            try
            {
                var drives = DriveInfo.GetDrives();
                foreach (var drive in drives)
                {
                    if (drive.IsReady)
                    {
                        if (!string.Equals(drive.Name, "/", StringComparison.OrdinalIgnoreCase)) continue;
                        return new DiagnosticsType()
                        {
                            Name = drive?.Name ?? "",
                            TotalSize = (drive?.TotalSize ?? 0) / (1024 * 1024 * 1024),
                            AvailableSize = (drive?.AvailableFreeSpace ?? 0) / (1024 * 1024 * 1024)
                        };
                    }
                }
                return new DiagnosticsType();
            }
            catch (Exception ex)
            {
                if (!string.Equals(ex?.Message, "Access to the path is denied.", StringComparison.OrdinalIgnoreCase)) throw new Exception(ex?.Message);
                throw new Exception(ex?.Message);
            }
        }
        #endregion
    }
}
