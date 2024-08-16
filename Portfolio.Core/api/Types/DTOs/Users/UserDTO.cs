using System.Runtime.Serialization;
using Portfolio.Core.Interfaces.Context.Users;
using Portfolio.Core.Types.Enums.Users;
using Portfolio.Core.Utils.DefaultUtils;
namespace Portfolio.Core.Types.DTOs.Users
{
    [DataContract]
    public class UserDTO : IUserProperties
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "confirmPassword")]
        public string ConfirmPassword { get; set; }

        [DataMember(Name = "phone")]
        public string Phone { get; set; }

        [DataMember(Name = "role")]
        public UserRoleEnum RoleEnum { get; set; }
        public string Role => RoleEnum.GetStringValue();

        [DataMember(Name = "status")]
        public UserStatusEnum StatusEnum { get; set; }
        public string Status => StatusEnum.GetStringValue();

    }
}