using System.Security.Cryptography;
using System.Text;

namespace Portfolio.Core.Utils.SecurityService
{
    public static class SecurityService
    {
        public static byte[] GenerateSalt()
        {
            using (var rng = RandomNumberGenerator.Create()) // This method creates an instance of a default cryptographic random number generator that can be used to fill a byte array with cryptographically secure random bytes.
            {
                byte[] salt = new byte[4]; // [Default:16] Adjust the size based on your security requirements
                rng.GetBytes(salt); // This fills the byte array salt with a sequence of random values which are cryptographically strong.
                return salt;
            }
        }

        // @USE: Check if the password is different from empty string or null
        public static string HashPassword(string password, byte[] salt)
        {
            if (string.IsNullOrEmpty(password) || salt == null || salt?.Length <= 0) return null;
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

                // Concatenate password and salt
                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

                // Hash the concatenated password and salt
                byte[] hashedBytes = sha256.ComputeHash(saltedPassword);

                // Concatenate the salt and hashed password for storage
                byte[] hashedPasswordWithSalt = new byte[hashedBytes.Length + salt.Length];
                Buffer.BlockCopy(salt, 0, hashedPasswordWithSalt, 0, salt.Length);
                Buffer.BlockCopy(hashedBytes, 0, hashedPasswordWithSalt, salt.Length, hashedBytes.Length);

                return Convert.ToBase64String(hashedPasswordWithSalt);
            }
        }
    }
}