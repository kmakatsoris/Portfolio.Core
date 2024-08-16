using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Portfolio.Core.Interfaces.Context.Users;
using Portfolio.Core.Types.Context;
using Portfolio.Core.Types.Enums.Users;
using Portfolio.Core.Types.Models.Users;
using Portfolio.Core.Utils.DefaultUtils;
using Portfolio.Core.Utils.UsersUtils;
using ILogger = NLog.ILogger;

// @TAG: DB_ENTITIES
namespace Portfolio.Core.Services.Context
{
    public class UserRepository : IUserRepository
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ILogger _logger;

        public UserRepository(
            IOptions<AppSettings> appSettings,
            ILogger logger
            )
        {
            _appSettings = appSettings;
            _logger = logger;
        }

        #region Fetch Methods [R from CRUD]
        // Fetch All Users,
        public async Task<IEnumerable<User>> GetAllUsersAsync(string[] propertiesToHide)
        {
            var sql = "SELECT * FROM Users";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.QueryAsync<User>(sql);
                    return result?.ToPerformUsersFromDB(propertiesToHide);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting all users. @GetAllUsersAsync");
                throw new Exception(ex?.Message);
            }
        }

        // Fetch User with Specific User ID,
        public async Task<IEnumerable<User>> GetUserByEmailAsync(string email, string[] propertiesToHide = null)
        {
            if (propertiesToHide == null)
                propertiesToHide = new string[] { "Id", "Password", "ConfirmPassword", "Token", "Salt" };
            if (string.IsNullOrEmpty(email)) return new List<User>();
            var sql = "SELECT * FROM Users WHERE email = @Email";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    IEnumerable<User> result = await connection.QueryAsync<User>(sql, new { Email = email });
                    return result?.ToPerformUsersFromDB(propertiesToHide);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting user with Email:[{email}]. @GetUserByEmailAsync");
                throw new Exception(ex?.Message);
            }
        }

        // Fetch User and check Status
        public async Task<bool> HasStatus(IEnumerable<User> users, IEnumerable<UserStatusEnum> statuses)
        {
            try
            {
                if (users != null && users?.Count() == 1)
                {
                    User u = users?.SingleOrDefault();
                    return statuses.Any(status => status.ToString() == u?.Status);
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error checking statuses. @HasStatus");
                throw new Exception(ex?.Message);
            }
        }

        #endregion

        #region Create Methods [C from CRUD]
        public async Task<bool> AddUserAsync(User user)
        {
            string sql = @"
                INSERT INTO Users (Username, Email, Password, ConfirmPassword, Phone, Salt, Token, Role, Status) 
                VALUES (@Username, @Email, @Password, @ConfirmPassword, @Phone, @Salt, @Token, @Role, @Status)";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var parameters = new
                    {
                        user.Username,
                        user.Email,
                        user.Password,
                        user.ConfirmPassword,
                        user.Phone,
                        user.Salt,
                        user.Token,
                        Role = user.Role.ToString(),
                        Status = user.Status.ToString()
                    };
                    var result = await connection.ExecuteAsync(sql, parameters);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error adding new user. @AddUserAsync");
                throw new Exception(ex?.Message);
            }
        }
        #endregion

        #region Update Methods [U from CRUD]
        public async Task<bool> UpdateUserAllDataAsync(User user)
        {
            if (user == null) return false;
            var sql = "UPDATE Users SET Username = @Username, Email = @Email, Password = @Password, ConfirmPassword = @ConfirmPassword, Phone = @Phone, Salt = @Salt, Token = @Token, Role = @Role, Status = @Status WHERE Email = @Email";
            // string testSql = $"UPDATE Users SET Username = {user?.Username}, Email = {user?.Email}, Password = {user?.Password}, ConfirmPassword = {user?.ConfirmPassword}, Phone = {user?.Phone}, Salt = {user?.Salt}, Token = {user?.Token}, Role = {user?.Role}, Status = {user?.Status} WHERE Email = {user?.Email}";

            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.ExecuteAsync(sql, user);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating user. @UpdateUserAsync");
                throw new Exception(ex?.Message);
            }
        }

        public async Task<bool> UpdateUserTokenAsync(User user)
        {
            var sql = "UPDATE Users SET Token = @Token WHERE Email = @Email";

            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.ExecuteAsync(sql, user);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating user. @UpdateUserAsync");
                throw new Exception(ex?.Message);
            }
        }

        public async Task<bool> UpdateUserBasicDataAsync(User user)
        {
            var sql = "UPDATE Users SET Username = @Username, Email = @Email, Phone = @Phone, Status = @Status WHERE Email = @Email";

            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.ExecuteAsync(sql, user);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating user. @UpdateUserAsync");
                throw new Exception(ex?.Message);
            }
        }

        public async Task<bool> UpdateStatusAsync(User user)
        {
            var sql = "UPDATE Users SET Status = @Status WHERE Email = @Email";
            // string testSql = $"UPDATE Users SET Status = {user?.Status} WHERE Email = {user?.Email}";

            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.ExecuteAsync(sql, user);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating user. @UpdateStatusAsync");
                throw new Exception(ex?.Message);
            }
        }

        public async Task<bool> LogoutUserAsync(string email)
        {
            User user = new User { Email = email, Status = UserStatusEnum.LoggedOut.GetStringValue(), Token = "" };
            var sql = "UPDATE Users SET status = @Status, token=@Token WHERE email = @Email";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.ExecuteAsync(sql, user);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating user. @LogoutUserAsync");
                throw new Exception(ex?.Message);
            }
        }
        #endregion

        #region Delete Methods [D from CRUD]
        public async Task<bool> DeleteUserAsync(string email)
        {
            var sql = "DELETE FROM Users WHERE Email = @Email";
            try
            {
                using (var connection = new MySqlConnection(_appSettings?.Value?.ConnectionStrings?.IdentityConnection ?? ""))
                {
                    await connection.OpenAsync();
                    var result = await connection.ExecuteAsync(sql, new { Email = email });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting user. @DeleteUserAsync");
                throw new Exception(ex?.Message);
            }
        }
        #endregion
    }

}