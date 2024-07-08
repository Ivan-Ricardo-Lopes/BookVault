using System.Security.Cryptography;

namespace BallastLane.BookVault.Application.Features.Users.Helpers
{
    internal static class PasswordHelper
    {
        public static string GenerateRandomSalt(int saltSize = 32)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] saltBytes = new byte[saltSize];
                rng.GetBytes(saltBytes);

                string salt = Convert.ToBase64String(saltBytes);

                return salt;
            }
        }
        public static string ComputeHash(string password, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);

            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                byte[] saltedPassword = new byte[passwordBytes.Length + saltBytes.Length];

                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(saltBytes, 0, saltedPassword, passwordBytes.Length, saltBytes.Length);

                byte[] hashBytes = sha256.ComputeHash(saltedPassword);
                string hash = Convert.ToBase64String(hashBytes);

                return hash;
            }
        }

        public static bool VerifyPassword(string password, string salt, string expectedHash)
        {
            string hash = ComputeHash(password, salt);

            return hash == expectedHash;
        }
    }
}
