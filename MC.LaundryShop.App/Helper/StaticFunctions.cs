using System.Security.Cryptography;
using System.Text;

namespace MC.LaundryShop.App.Helper
{
    public static class StaticFunctions
    {
        public static string ComputeSha256Hash(string passwordString)
        {
            using (var sha256Hash = SHA256.Create())
            {
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(passwordString));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
