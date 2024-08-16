using System.Runtime.Serialization;

namespace Portfolio.Core.Types.DTOs.Requests.Identity
{
    [DataContract]
    public class IdentityBaseRequest : BaseRequest
    {
        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "confirmPassword")]
        public string ConfirmPassword { get; set; }
    }
}