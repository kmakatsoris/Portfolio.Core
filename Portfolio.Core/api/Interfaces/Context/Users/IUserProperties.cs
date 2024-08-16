using Portfolio.Core.Types.Enums.Users;

namespace Portfolio.Core.Interfaces.Context.Users
{
    public interface IUserProperties
    {
        string Username { get; }
        string Email { get; }
        string Password { get; }
        string ConfirmPassword { get; }
        string Phone { get; }
        string Role { get; }
        string Status { get; }
    }
}