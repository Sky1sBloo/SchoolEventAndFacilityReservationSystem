using BCrypt.Net;

namespace SFERS.Utilities
{
    public static class PasswordManager
    {
        // Hash a plain text password
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        // Verify a plain text password against a hashed password
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}