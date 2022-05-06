using Andy.X.Client.Abstractions;
using Andy.X.Client.Configurations;

namespace Andy.X.Client.Factories
{
    public class XClientFactory : XClientConfiguration, IXClientFactory
    {
        private readonly XClientConfiguration _configuration;
        public XClientFactory(XClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        public XClient CreateClient()
        {
            return XClient.CreateConnection(_configuration);
        }
    }
}
