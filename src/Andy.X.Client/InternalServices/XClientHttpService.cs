using Andy.X.Client.Configurations;
using Andy.X.Client.Models.Internal;
using Andy.X.Client.Utilities;
using System;
using System.Net.Http;
using System.Text.Json;

namespace Andy.X.Client.InternalServices
{
    internal class XClientHttpService
    {
        private readonly XClientConfiguration _xClientConfiguration;
        private HttpClient _httpClient;
        public XClientHttpService(XClientConfiguration xClientConfiguration)
        {
            _xClientConfiguration = xClientConfiguration;
        }

        public XClientConnection GetApplicationDetails()
        {
            _httpClient = new HttpClient(_xClientConfiguration.Settings.HttpClientHandler);
            _httpClient.DefaultRequestHeaders.Add("x-called-by", ApplicationParameters.LibraryName);

            string request = $"{_xClientConfiguration.NodeUrl.AbsoluteUri}api/v3/node/version";

            try
            {
                HttpResponseMessage httpResponseMessage = _httpClient.GetAsync(request).Result;
                string content = httpResponseMessage.Content.ReadAsStringAsync().Result;
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var applicationDetails = JsonSerializer.Deserialize<ApplicationDetails>(content,
                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                    return new XClientConnection(XConnectionState.Open, applicationDetails.Name, applicationDetails.Version);
                }

                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    httpResponseMessage.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    throw new Exception("Credentials of Andy X are not correct");

            }
            catch (Exception)
            {

            }

            return null;
        }
    }
}
