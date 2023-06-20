using BCrypt.Net;
using Microsoft.AspNetCore.Components.Web;

namespace HotMusic.Common
{
    public class HashPass
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        public static bool VerifyPassword(string password,string hashedPassword) {
            bool verify = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            return verify;
        }
    }
}
