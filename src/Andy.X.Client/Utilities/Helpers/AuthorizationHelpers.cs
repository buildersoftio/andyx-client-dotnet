using System;
using System.Text;

namespace Andy.X.Client.Utilities.Helpers
{
    internal static class AuthorizationHelpers
    {
        public static string GenerateToken(string key, string secret)
        {
            string encodedPassword = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
               .GetBytes(key + ":" + secret));

            return encodedPassword;
        }
    }
}
