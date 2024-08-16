namespace Portfolio.Core.Interfaces.Context
{
    public interface IMetaDataProperties
    {
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Data { get; }
    }
}