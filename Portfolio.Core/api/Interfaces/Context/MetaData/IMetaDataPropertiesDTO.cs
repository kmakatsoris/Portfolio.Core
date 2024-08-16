namespace Portfolio.Core.Interfaces.Context
{
    public interface IMetaDataPropertiesDTO<T> : IMetaDataProperties
    {
        public T DataJSON { get; set; }
    }
}