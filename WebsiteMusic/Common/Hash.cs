using BCryptNet = BCrypt.Net.BCrypt;

namespace WebsiteMusic.Common
{
    public class Hash
    {
        public static string HashPassword(string password)
        {
            string hashedPassword = BCryptNet.HashPassword(password);
            return hashedPassword;
        }
        public static bool VerifyPassword(string password,string hashedPassword) {
            bool passwordMatch = BCryptNet.Verify(password, hashedPassword);
            return passwordMatch;
        }
    }
}
