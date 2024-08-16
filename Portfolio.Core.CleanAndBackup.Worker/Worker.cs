using Microsoft.Extensions.Options;
using NLog;
using Portfolio.Core.CleanAndBackup.Worker.Types;
using Portfolio.Core.CleanAndBackup.Worker.Workers;
using ILogger = NLog.ILogger;

namespace Portfolio.Core.CleanAndBackup.Worker
{
    public class Worker : BackgroundService
    {
        private const int DEFAULT_PREVENT_STORAGE_COLLAPSE_WORKER_INTERVAL = 300; // 5 minutes (seconds)
        private const int DEFAULT_BACKUP_WORKER_INTERVAL = 604800; // 7 Days (seconds)
        private const int DEFAULT_CLEANUP_WORKER_INTERVAL = 604800; // 7 Days (seconds)

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IOptions<AppSettings> _appSettings;

        public Worker(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // Calculate intervals
                int preventStorageCollapseInterval = _appSettings?.Value?.PreventStorageCollapseWorkerConfig?.IntervalTime ?? DEFAULT_PREVENT_STORAGE_COLLAPSE_WORKER_INTERVAL;
                int backupWorkerInterval = _appSettings?.Value?.BackupWorkerConfig?.IntervalTime ?? DEFAULT_BACKUP_WORKER_INTERVAL;
                int cleanupWorkerInterval = _appSettings?.Value?.CleanupWorkerConfig?.IntervalTime ?? DEFAULT_CLEANUP_WORKER_INTERVAL;

                // Initialize next run times
                DateTimeOffset nextPreventStorageCollapseRun = DateTimeOffset.Now.AddSeconds(preventStorageCollapseInterval);
                DateTimeOffset nextBackupRun = DateTimeOffset.Now.AddSeconds(backupWorkerInterval);
                DateTimeOffset nextCleanupRun = DateTimeOffset.Now.AddSeconds(cleanupWorkerInterval);

                while (!stoppingToken.IsCancellationRequested)
                {
                    DateTimeOffset now = DateTimeOffset.Now;

                    if (now >= nextPreventStorageCollapseRun)
                    {
                        await ExecuteAsync_PreventStorageCollapse_Worker(stoppingToken);
                        nextPreventStorageCollapseRun = now.AddSeconds(preventStorageCollapseInterval);
                    }

                    if (now >= nextBackupRun)
                    {
                        await ExecuteAsync_BackUp_Worker(stoppingToken);
                        nextBackupRun = now.AddSeconds(backupWorkerInterval);
                    }

                    if (now >= nextCleanupRun)
                    {
                        await ExecuteAsync_CleanUp_Worker(stoppingToken);
                        nextCleanupRun = now.AddSeconds(cleanupWorkerInterval);
                    }

                    // Determine the smallest interval to delay before checking again
                    var nextRunTime = new[] { nextPreventStorageCollapseRun, nextBackupRun, nextCleanupRun }.Min();
                    var delay = nextRunTime - now;

                    if (delay > TimeSpan.Zero)
                    {
                        await Task.Delay(delay, stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while executing the worker tasks.");
                throw;
            }
        }

        private async Task ExecuteAsync_PreventStorageCollapse_Worker(CancellationToken stoppingToken)
        {
            _logger.Warn("Starting PreventStorageCollapseWorker...");
            var worker = new PreventStorageCollapseWorker(_appSettings, new Guid("FC7BFDC5-628B-4B31-AC0E-8690D83A8C7B"));
            worker.Run();
            await Task.CompletedTask; // Simulate async operation if necessary
        }

        private async Task ExecuteAsync_BackUp_Worker(CancellationToken stoppingToken)
        {
            _logger.Debug("Starting BackUpWorker...");
            var worker = new DefaultWorker(new Guid("C0191148-C50C-4981-87C7-106F3FBB6216"), _appSettings?.Value?.BackupWorkerConfig?.DefaultBashScriptPath ?? "");
            worker.Run();
            await Task.CompletedTask; // Simulate async operation if necessary
        }

        private async Task ExecuteAsync_CleanUp_Worker(CancellationToken stoppingToken)
        {
            _logger.Warn("Starting CleanUpWorker...");
            var worker = new DefaultWorker(new Guid("F9D8EA42-1D4C-40FC-A345-2D83798282FA"), _appSettings?.Value?.CleanupWorkerConfig?.DefaultBashScriptPath ?? "");
            worker.Run();
            await Task.CompletedTask; // Simulate async operation if necessary
        }
    }
}
