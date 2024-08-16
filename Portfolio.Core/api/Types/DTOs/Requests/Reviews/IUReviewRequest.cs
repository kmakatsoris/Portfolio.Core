using System.Runtime.Serialization;
using Portfolio.Core.Types.DTOs.Requests.Identity;
using Portfolio.Core.Types.DTOs.Reviews;

namespace Portfolio.Core.Types.DTOs.Requests
{
    [DataContract]
    public class IUReviewRequest : BaseRequest
    {
        [DataMember(Name = "review")]
        public ReviewDTO Review { get; set; }
    }

    [DataContract]
    public class IUReviewSingleDataRequest : BaseRequest
    {
        [DataMember(Name = "review")]
        public MetaDataDTO<ReviewData> Review { get; set; }
    }
}