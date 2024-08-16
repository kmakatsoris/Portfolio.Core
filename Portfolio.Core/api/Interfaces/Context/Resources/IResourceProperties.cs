namespace Portfolio.Core.Interfaces.Context.Resources
{
    public interface IResourceProperties
    {
        public string Title { get; }
        public string BriefDescription { get; }
        public string ExtensiveDescription { get; }
        public Guid MetaDataID { get; }
        public string RenderPath { get; }
        public string StoragePath { get; }
        public string PreviewData { get; }
        public string Dimension { get; }
        public string Tags { get; }
        public string Type { get; }
        public DateTime CreatedAt { get; }
        public DateTime UpdatedAt { get; }
    }
}