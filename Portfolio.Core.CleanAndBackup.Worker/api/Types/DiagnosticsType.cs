namespace Portfolio.Core.CleanAndBackup.Worker.Types {
    public class DiagnosticsType
    {
        public string Name { get; set; }
        public long TotalSize { get; set; }
        public long AvailableSize { get; set; }
        public long UsedSize => (TotalSize - AvailableSize) / (1024 * 1024 * 1024);
        
        public override string ToString()
        {
            string diagTxt = "";
            diagTxt += $"Drive {Name}:";
            diagTxt += $"  Total Size: {TotalSize} GB";
            diagTxt += $"  Available Space: {AvailableSize} GB";
            diagTxt += $"  Used Space: {UsedSize} GB";
            return diagTxt;
        }        
    }
}