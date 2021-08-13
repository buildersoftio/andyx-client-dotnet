using Microsoft.Extensions.Logging;

namespace Andy.X.Client.Configurations
{
    public class XClientConfiguration
    {
        public string XNodeUrl { get; set; }
        public string Token { get; set; }

        public string Tenant { get; set; }
        public string Product { get; set; }

        public XClientLogging Logging { get; set; }

        public XClientConfiguration()
        {
            XNodeUrl = "https://localhost:9001";
            Tenant = "default";
            Product = "default";
            Logging = new XClientLogging();
        }

        public XClientConfiguration(string xNodeUrl)
        {
            XNodeUrl = xNodeUrl;
            Tenant = "default";
            Product = "default";
            Logging = new XClientLogging();
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
