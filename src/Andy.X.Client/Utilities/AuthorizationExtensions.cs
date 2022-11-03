using System;
using System.Net.Http;
using System.Text;

namespace Andy.X.Client.Utilities
{
    internal static class AuthorizationExtensions
    {
        internal static void AddBasicAuthorizationHeader(this HttpClient httpClient, string username, string password)
        {
            string encodedPassword = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
               .GetBytes(username + ":" + password));

            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + encodedPassword);
        }
    }
}
