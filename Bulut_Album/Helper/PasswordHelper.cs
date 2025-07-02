namespace Bulut_Album.Helper
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string password, string hashed)
        {
            return HashPassword(password) == hashed;
        }
    }

}
