using System.Runtime.Serialization;

namespace Portfolio.Core.Types.DTOs.Requests
{
    [DataContract]
    public class BaseRequest
    {
        [DataMember(Name = "email")]
        public string Email { get; set; }
    }
}