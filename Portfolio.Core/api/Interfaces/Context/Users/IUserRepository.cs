using Portfolio.Core.Types.Enums.Users;
using Portfolio.Core.Types.Models.Users;

namespace Portfolio.Core.Interfaces.Context.Users
{
    public interface IUserRepository
    {
        // Fetch Methods [R from CRUD]
        Task<IEnumerable<User>> GetAllUsersAsync(string[] propertiesToHide);
        Task<IEnumerable<User>> GetUserByEmailAsync(string email, string[] propertiesToHide = null);
        Task<bool> HasStatus(IEnumerable<User> users, IEnumerable<UserStatusEnum> statuses);

        // Insert/Create Methods [C from CRUD]
        Task<bool> AddUserAsync(User user);

        // Update/Edit Methods [U from CRUD]
        Task<bool> UpdateUserAllDataAsync(User user);
        Task<bool> UpdateUserTokenAsync(User user);
        Task<bool> UpdateStatusAsync(User user);

        Task<bool> LogoutUserAsync(string email);

        // Delete/Remove Methods [D from CRUD]
        Task<bool> DeleteUserAsync(string email);
    }
}