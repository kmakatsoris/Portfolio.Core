namespace Portfolio.Core.CleanAndBackup.Worker.Types {    
    public class AppSettings
    {        
        public PreventStorageCollapseWorkerConfig PreventStorageCollapseWorkerConfig { get; set; }
        public BackupWorkerConfig BackupWorkerConfig { get; set; }
        public CleanupWorkerConfig CleanupWorkerConfig { get; set; }
    }

    public class PreventStorageCollapseWorkerConfig
    {
        public int IntervalTime { get; set; }
        public int MinAvailableSizeGB { get; set; }
        public string MinAvailableSizeGBBashScriptPath { get; set; }
    }

    public class BackupWorkerConfig 
    {
        public int IntervalTime { get; set; }
        public string DefaultBashScriptPath { get; set; }
    }

    public class CleanupWorkerConfig 
    {
        public int IntervalTime { get; set; }
        public string DefaultBashScriptPath { get; set; }
    }
}    
