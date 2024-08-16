using System.Runtime.Serialization;

namespace Portfolio.Core.Types.DTOs.Responses
{
    [DataContract]
    public class BaseResponse
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }
    }
}