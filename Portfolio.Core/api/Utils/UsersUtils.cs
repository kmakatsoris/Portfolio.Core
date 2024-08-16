using Microsoft.Extensions.Primitives;
using Portfolio.Core.Types.Enums.Users;
using Portfolio.Core.Types.Models.Users;
using Portfolio.Core.Utils.DefaultUtils;

namespace Portfolio.Core.Utils.UsersUtils
{
    public static class UsersUtils
    {
        // @TAG: DB_ENTITIES
        public static User ToPerformUsersFromDB(this User user, string[] propertiesToHide)
        {
            return new User
            {
                Id = propertiesToHide?.Contains("Id") == true ? Guid.Empty : user?.Id ?? Guid.Empty,
                Password = propertiesToHide?.Contains("Password") == true ? "*****" : user?.Password ?? "",
                ConfirmPassword = propertiesToHide?.Contains("ConfirmPassword") == true ? "*****" : user?.ConfirmPassword ?? "",
                Token = propertiesToHide?.Contains("Token") == true ? "*****" : user?.Token ?? "",
                Username = propertiesToHide?.Contains("Username") == true ? "*****" : user?.Username ?? "",
                Email = propertiesToHide?.Contains("Email") == true ? "*****" : user?.Email ?? "",
                Phone = propertiesToHide?.Contains("Phone") == true ? "*****" : user?.Phone ?? "",
                Salt = propertiesToHide?.Contains("Salt") == true ? new byte[0] : user?.Salt ?? new byte[0],
                Role = propertiesToHide?.Contains("Role") == true ? UserRoleEnum.DEFAULT.GetStringValue() : user?.Role.ToString() ?? UserRoleEnum.DEFAULT.GetStringValue(),
                Status = propertiesToHide?.Contains("Status") == true ? UserStatusEnum.DEFAULT.GetStringValue() : user?.Status.ToString() ?? UserStatusEnum.DEFAULT.GetStringValue(),
                CreatedAt = propertiesToHide?.Contains("CreatedAt") == true ? DateTime.UtcNow : user?.CreatedAt ?? DateTime.UtcNow
            };
        }

        // @TAG: DB_ENTITIES
        public static IEnumerable<User> ToPerformUsersFromDB(this IEnumerable<User> users, string[] propertiesToHide)
        {
            if (users == null || users.Count() <= 0) return new List<User>();
            var usersToReturn = new List<User>();
            foreach (var user in users)
            {
                usersToReturn.Add(user?.ToPerformUsersFromDB(propertiesToHide));
            }
            return usersToReturn;
        }

        public static string ExtractToken(this IHeaderDictionary Headers)
        {
            if (Headers == null) return "";
            StringValues authorizationValue = new StringValues();
            string[] tokens = Headers.TryGetValue("Authorization", out authorizationValue) == true && !StringValues.IsNullOrEmpty(authorizationValue) ? authorizationValue.FirstOrDefault().ToString().Split(" ") : [];

            string token = "";
            if (tokens != null && tokens?.Length >= 1)
            {
                if (tokens?.Length == 1)
                {
                    token = tokens[0];
                }
                else
                {
                    if (tokens.Contains("Bearer"))
                    {
                        token = tokens[1];
                    }
                    else
                    {
                        return "";
                    }
                }
            }

            return token;
        }

    }
}