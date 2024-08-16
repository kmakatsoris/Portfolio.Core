using System.Runtime.Serialization;
using Newtonsoft.Json;
using Portfolio.Core.Interfaces.Context;

namespace Portfolio.Core.Types.DTOs
{
    [DataContract]
    public class MetaDataDTO<T> : IMetaDataPropertiesDTO<T>
    {
        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "createdAt")]
        public DateTime CreatedAt { get; set; }

        [DataMember(Name = "updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [DataMember(Name = "data")]
        public T DataJSON { get; set; }
        public string Data => JsonConvert.SerializeObject(DataJSON);
    }
}