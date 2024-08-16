using System.Text;
using Portfolio.Core.Interfaces.Context;
using Portfolio.Core.Interfaces.Context.Users;
using Portfolio.Core.Types.DataTypes;
using Portfolio.Core.Types.DTOs.Users;
using Portfolio.Core.Types.Models.Users;
using Portfolio.Core.Utils.SecurityService;

namespace Portfolio.Core.Implementation.Identity
{
    public partial class AuthenticationService
    {
        // @TODO: Add Regex Validations Synced with Users DB
        // Validation for users to be considered valid.
        private bool IsDefaultUserValid<T>(T user) where T : IUserProperties
        {
            return user != null
                   && !string.IsNullOrEmpty(user.Email) && user.Email.Length <= 255
                   && !string.IsNullOrEmpty(user.Password) && user.Password.Length <= 255
                   && !string.IsNullOrEmpty(user.ConfirmPassword) && user.ConfirmPassword.Length <= 255;
        }

        // Validation for users to verify the uniqueness of their email address.
        private async Task<bool> IsEmailUnique(string email)
        {
            if (string.IsNullOrEmpty(email) || _userRepository == null) return false;
            IEnumerable<User> user = await _userRepository?.GetUserByEmailAsync(email, ["Id", "Password", "ConfirmPassword", "Token", "Salt"]);
            return user == null || user?.Count() <= 0;
        }

        // Check if a IEnumerable Is Single
        private bool IsIEnumerableSingle<T>(IEnumerable<T> e, out T obj)
        {
            bool validation = e != null && e?.Count() == 1;
            obj = default;
            if (validation == true)
                obj = e.SingleOrDefault();
            return validation && obj != null;
        }

        // Check if a IEnumerable Is Null or Empty
        private bool IsIEnumerableNullOrEmpty<T>(IEnumerable<T> e) => e == null || e?.Count() <= 0;

        // Validate the Request's Password
        private bool IsPasswordValid(PasswordValidationType pvt)
        {
            if (pvt == null) return false;
            string storedHashedPassword = pvt?.UserConfirmPassword;// "hashed_password_from_database";
            byte[] storedSaltBytes = pvt?.UserSalt;
            string enteredPassword = pvt?.RequestConfirmPassword; //"user_entered_password";

            // Convert the stored salt and entered password to byte arrays            
            byte[] enteredPasswordBytes = Encoding.UTF8.GetBytes(enteredPassword);

            // Concatenate entered password and stored salt
            byte[] saltedPassword = new byte[enteredPasswordBytes.Length + storedSaltBytes.Length];
            Buffer.BlockCopy(enteredPasswordBytes, 0, saltedPassword, 0, enteredPasswordBytes.Length);
            Buffer.BlockCopy(storedSaltBytes, 0, saltedPassword, enteredPasswordBytes.Length, storedSaltBytes.Length);

            // Hash the concatenated value
            string enteredPasswordHash = SecurityService.HashPassword(enteredPassword, storedSaltBytes);

            return enteredPasswordHash?.ToLower().Equals(storedHashedPassword?.ToLower()) == true;
        }

    }
}