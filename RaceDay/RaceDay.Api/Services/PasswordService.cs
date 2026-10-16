namespace RaceDay.Api.Services
{
    public static class PasswordService
    {
        // Hashes a plain-text password using BCrypt
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Verifies a plain-text password against the stored database hash
        public static bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}