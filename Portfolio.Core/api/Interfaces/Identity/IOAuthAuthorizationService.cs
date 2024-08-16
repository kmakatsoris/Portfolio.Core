using Portfolio.Core.Types.DTOs.Users;
using Portfolio.Core.Types.Models.Users;

namespace Portfolio.Core.Interfaces.Identity
{
    public interface IOAuthAuthorizationService
    {
        Task<string> GenerateJwtToken(User user);
        Task<bool> ValidateUserJwtToken(string email, string token);
        string GetEmailFromToken(string token);
    }
}