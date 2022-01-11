using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Andy.X.Client.Configurations
{
    public class XClientConfiguration
    {
        public string ServiceUrl { get; set; }

        public string TenantToken { get; set; }

        public string Tenant { get; set; }
        public string Product { get; set; }

        public bool AutoConnect { get; set; }

        public HttpClientHandler HttpClientHandler { get; set; }

        public XClientLogging Logging { get; set; }

        /// <summary>
        /// Initialize XClient
        /// </summary>
        public XClientConfiguration()
        {
            ServiceUrl = "https://localhost:9001";
            Tenant = "default";
            Product = "default";
            AutoConnect = true;
            Logging = new XClientLogging();
            HttpClientHandler = new HttpClientHandler();
        }

        /// <summary>
        /// Initialize XClient
        /// </summary>
        /// <param name="serviceUrl">serviceUrl, default value is https://localhost:9001</param>
        public XClientConfiguration(string serviceUrl)
        {
            ServiceUrl = serviceUrl;
            Tenant = "default";
            Product = "default";
            AutoConnect = true;
            Logging = new XClientLogging();
            HttpClientHandler = new HttpClientHandler();
        }
    }

    public class XClientLogging
    {
        private readonly ILoggerFactory _loggerFactory = null;

        public XClientLogging()
        {
            _loggerFactory = new LoggerFactory();
        }

        public XClientLogging(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public ILoggerFactory GetLoggerFactory()
        {
            return _loggerFactory;
        }
    }
}
