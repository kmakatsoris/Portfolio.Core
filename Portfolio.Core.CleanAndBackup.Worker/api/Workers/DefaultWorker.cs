using System;
using System.IO;
using Microsoft.Extensions.Options;
using NLog;
using Portfolio.Core.CleanAndBackup.Worker.Types;
using Portfolio.Core.CleanAndBackup.Worker.Utils;
using ILogger = NLog.ILogger;

namespace Portfolio.Core.CleanAndBackup.Worker.Workers
{
    public class DefaultWorker
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly Guid _PID;
        private readonly string bashScriptpath;

        public DefaultWorker(Guid pid, string shPath)
        {
            bashScriptpath = shPath;
            _PID = pid;
        }
        public void Run()
        {
            try
            {
                DefaultUtils.ExecuteBashScript(_logger, bashScriptpath);
            }
            catch (Exception ex)
            {
                _logger.Error($"@Important: Error executing the Run Command (PID:{_PID}) with exception message: {ex?.Message}");
            }
        }
    }
}
