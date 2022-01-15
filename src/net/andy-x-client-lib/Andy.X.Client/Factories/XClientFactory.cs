using Andy.X.Client.Abstractions;
using Andy.X.Client.Configurations;

namespace Andy.X.Client.Factories
{
    public class XClientFactory: XClientConfiguration, IXClientFactory
    {
        private readonly XClientConfiguration configuration;
        public XClientFactory(XClientConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public XClient CreateClient()
        {
            return new XClient(configuration);
        }
    }
}
