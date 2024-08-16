using System.Data;
using System.Text;
using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Portfolio.Core.Interfaces.Context;
using Portfolio.Core.Interfaces.Context.Users;
using Portfolio.Core.Interfaces.Identity;
using Portfolio.Core.Types.Context;
using Portfolio.Core.Types.DataTypes;
using Portfolio.Core.Types.DTOs.Requests.Identity;
using Portfolio.Core.Types.DTOs.Users;
using Portfolio.Core.Types.Enums.Users;
using Portfolio.Core.Types.Models.Users;
using Portfolio.Core.Utils.DefaultUtils;

namespace Portfolio.Core.Implementation.Identity
{
    public partial class AuthenticationService : IAuthenticationService
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IUserRepository _userRepository;
        private readonly IOAuthAuthorizationService _oauthService;

        public AuthenticationService(
            IOptions<AppSettings> appSettings,
            IUserRepository userRepository,
            IOAuthAuthorizationService oauthService)
        {
            _appSettings = appSettings;
            _userRepository = userRepository;
            _oauthService = oauthService;
        }

        public async Task<bool> RegisterAsync(UserDTO userDTO, bool isAdminFlag = false)
        {
            // Check if the username is already taken and Validation Checks. 
            IEnumerable<User> users = await _userRepository.GetUserByEmailAsync(userDTO?.Email, ["Id", "Password", "ConfirmPassword", "Token", "Salt"]);
            if (!IsDefaultUserValid<UserDTO>(userDTO) || !await IsEmailUnique(userDTO?.Email) || _userRepository == null || await _userRepository?.HasStatus(users, [UserStatusEnum.Blocked])) return false;

            userDTO.RoleEnum = isAdminFlag == true ? UserRoleEnum.Admin : UserRoleEnum.User;
            userDTO.StatusEnum = UserStatusEnum.Active;
            User insertUser = PreparePasswordChangeRequest(userDTO);

            // Insert the new user
            return await _userRepository?.AddUserAsync(insertUser);
        }

        public async Task<string> LoginAsync(UserDTO userDTO)
        {
            // Validations,
            IEnumerable<User> users = await _userRepository.GetUserByEmailAsync(userDTO?.Email, ["Id", "Password", "Token"]);
            if (!IsDefaultUserValid<UserDTO>(userDTO) || await _userRepository?.HasStatus(users, [UserStatusEnum.Blocked])) throw new Exception("Invalid Input.");

            // DB Validation
            if (IsIEnumerableNullOrEmpty(users) || !IsIEnumerableSingle(users, out User user) || !IsDefaultUserValid<User>(users?.SingleOrDefault())) throw new Exception("User not found.");

            return IsPasswordValid(new PasswordValidationType
            {
                UserConfirmPassword = user?.ConfirmPassword,
                UserSalt = user?.Salt,
                RequestConfirmPassword = userDTO?.ConfirmPassword
            }) ? await _userRepository?.UpdateStatusAsync(new User { Email = user?.Email, Status = UserStatusEnum.Active.GetStringValue() }) ? await _oauthService?.GenerateJwtToken(user) : "" : "";
        }

        public async Task<bool> LogoutAsync(string email)
        {
            bool success = await _userRepository?.LogoutUserAsync(email);
            // Perform cache actions,...
            return success;
        }

        public async Task<bool> IsLogoutAsync(string email)
        {
            IEnumerable<User> users = await _userRepository.GetUserByEmailAsync(email, ["Id", "Password", "ConfirmPassword", "Token", "Salt"]);
            return await _userRepository?.HasStatus(users, [UserStatusEnum.LoggedOut]);
        }

        public async Task<bool> ForgotPasswordAsync(IdentityBaseRequest request, bool isAdminFlag = false)
        {
            if (isAdminFlag == true) throw new Exception("We are sorry, but because you are an ADMIN user you need to contact first with the Portfolio Developer: kpmakatsoris@gmail.com");
            // Validations,
            IEnumerable<User> users = await _userRepository?.GetUserByEmailAsync(request?.Email, []);
            if (!IsIEnumerableSingle(users, out User user))
                return false;

            // @TODO: Implementation for password reset logic (e.g., sending a password reset email)              
            // Perform cache actions,...  

            // Make The Appropriate Modifications,
            user.Password = request?.Password;

            // Prepare the Update Request and Execute the user Update            
            return await _userRepository?.UpdateUserAllDataAsync(PreparePasswordChangeRequest(user));
        }

    }

}