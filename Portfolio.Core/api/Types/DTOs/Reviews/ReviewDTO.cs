using System.Runtime.Serialization;
using Portfolio.Core.Interfaces.Context;

namespace Portfolio.Core.Types.DTOs.Reviews
{
    [DataContract]
    public class ReviewDTO
    {
        [DataMember(Name = "metaData")]
        public MetaDataDTO<ReviewAllData> MetaData { get; set; }
    }
}