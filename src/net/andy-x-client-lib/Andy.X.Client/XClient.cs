using Andy.X.Client.Configurations;
using Microsoft.Extensions.Logging;

namespace Andy.X.Client
{
    public class XClient
    {
        private readonly XClientConfiguration xClientConfiguration;

        public XClient(XClientConfiguration xClientConfiguration)
        {
            this.xClientConfiguration = xClientConfiguration;
        }

        public XClient(string xNodeUrl)
        {
            xClientConfiguration = new XClientConfiguration(xNodeUrl);
        }

        public XClient Tenant(string tenant)
        {
            xClientConfiguration.Tenant = tenant;
            return this;
        }

        public XClient Product(string product)
        {
            xClientConfiguration.Product = product;
            return this;
        }

        public XClient Logging(ILoggerFactory loggerFactory)
        {
            xClientConfiguration.Logging = new XClientLogging(loggerFactory);
            return this;
        }

        public XClientConfiguration GetClientConfiguration()
        {
            return xClientConfiguration;
        }
    }
}
