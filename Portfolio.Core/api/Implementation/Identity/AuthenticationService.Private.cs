using Portfolio.Core.Interfaces.Context;
using Portfolio.Core.Interfaces.Context.Users;
using Portfolio.Core.Types.DTOs.Users;
using Portfolio.Core.Types.Enums.Users;
using Portfolio.Core.Types.Models.Users;
using Portfolio.Core.Utils.DefaultUtils;
using Portfolio.Core.Utils.SecurityService;

namespace Portfolio.Core.Implementation.Identity
{
    public partial class AuthenticationService
    {
        private User PreparePasswordChangeRequest<T>(T user) where T : IUserProperties
        {
            // Hash the password before storing it (implement your hashing logic here)
            byte[] saltBytes = SecurityService.GenerateSalt();
            string hashedPassword = SecurityService.HashPassword(user?.Password ?? "", saltBytes);
            string base64Salt = Convert.ToBase64String(saltBytes);

            byte[] retrievedSaltBytes = Convert.FromBase64String(base64Salt);

            // @TAG: DB_ENTITIES
            var insertUser = new User
            {
                Username = user?.Username ?? "",
                Email = user?.Email ?? "",
                Password = base64Salt,
                ConfirmPassword = hashedPassword,
                Phone = user?.Phone ?? "",
                Salt = retrievedSaltBytes,
                Token = "",
                Role = !string.IsNullOrEmpty(user?.Role) ? user?.Role : UserRoleEnum.DEFAULT.GetStringValue(),
                Status = !string.IsNullOrEmpty(user?.Status) ? user?.Status : UserStatusEnum.DEFAULT.GetStringValue(),
            };

            return insertUser;
        }
    }
}