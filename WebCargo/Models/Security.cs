using System.Security.Cryptography;
using System.Text;

namespace WebCargo.Models
{
    public static class Security
    {
        public static string HashPassword(
            string password)
        {
            using SHA256 sha =
                SHA256.Create();

            byte[] bytes =
                sha.ComputeHash(
                    Encoding.UTF8.GetBytes(password)
                );

            return Convert.ToHexString(bytes);
        }
    }
}