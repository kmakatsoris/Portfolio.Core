using Portfolio.Core.Types.DTOs.Requests.Identity;
using Portfolio.Core.Types.DTOs.Users;

namespace Portfolio.Core.Interfaces.Identity
{
    public interface IAuthenticationService
    {
        Task<bool> RegisterAsync(UserDTO userDTO, bool isAdminFlag = false);
        Task<string> LoginAsync(UserDTO userDTO);
        Task<bool> LogoutAsync(string email);
        Task<bool> ForgotPasswordAsync(IdentityBaseRequest request, bool isAdminFlag = false);
    }
}