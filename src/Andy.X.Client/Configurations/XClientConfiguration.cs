using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace Andy.X.Client.Configurations
{
    public class XClientConfiguration
    {
        public Uri NodeUrl { get; set; }
        public TenantConfiguration Tenant { get; set; }
        public ProductConfiguration Product { get; set; }

        public XClientSettings Settings { get; set; }

        public XClientConfiguration() : this(new Uri("http://localhost6540"))
        {
            // it calls the other constructor
        }

        /// <summary>
        /// Initialize XClientConfiguration
        /// </summary>
        /// <param name="serviceUrl">nodeUrl, default value is http://localhost:6540</param>
        public XClientConfiguration(Uri nodeUrl)
        {
            NodeUrl = nodeUrl;
            Tenant = new TenantConfiguration();
            Product = new ProductConfiguration();

            Settings = new XClientSettings();
        }
    }

    public class XClientSettings
    {
        public LoggingConfiguration Logging { get; set; }
        public HttpClientHandler HttpClientHandler { get; set; }

        public bool EnableAutoReconnect { get; set; }

        public int ConnectionTimeoutMs { get; set; }
        public int ReconnectionTimeoutMs { get; set; }


        public XClientSettings()
        {
            Logging = new LoggingConfiguration();
            HttpClientHandler = new HttpClientHandler();

            EnableAutoReconnect = true;
            ConnectionTimeoutMs = 10000;
            ReconnectionTimeoutMs = 1000;
        }
    }

    public class LoggingConfiguration
    {
        private readonly ILoggerFactory _loggerFactory = null;

        public LoggingConfiguration() : this(new LoggerFactory())
        {

        }

        public LoggingConfiguration(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public ILoggerFactory GetLoggerFactory()
        {
            return _loggerFactory;
        }
    }

}
