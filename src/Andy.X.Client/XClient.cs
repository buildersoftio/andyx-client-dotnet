using Andy.X.Client.Abstractions.XClients;
using Andy.X.Client.Configurations;
using System;

namespace Andy.X.Client
{
    public class XClient : IXClient
    {
        private readonly XClientConfiguration _configuration;

        private XClient()
        {
            _configuration = new XClientConfiguration();
        }

        private XClient(XClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static IXClientServiceConnection CreateClient()
        {
            return new XClient();
        }

        public static XClient CreateClient(XClientConfiguration configuration)
        {
            return new XClient(configuration);
        }

        public IXClientConfiguration AndProduct(string product)
        {
            _configuration.Product.Name = product;

            return this;
        }

        public IXClientConfiguration AndProduct(string product, string key, string secret)
        {
            _configuration.Product.Name = product;
            _configuration.Product.Key = key;
            _configuration.Product.Secret = secret;

            return this;
        }

        public IXClientProductConnection AndTenant(string tenant)
        {
            _configuration.Tenant.Name = tenant;

            return this;
        }

        public IXClientProductConnection AndTenant(string tenant, string key, string secret)
        {
            _configuration.Tenant.Name = tenant;
            _configuration.Tenant.Key = key;
            _configuration.Tenant.Secret = secret;

            return this;
        }

        public XClient Build()
        {
            return this;
        }

        public IXClientTenantConnection ForService(Uri nodeUrl)
        {
            _configuration.NodeUrl = nodeUrl;

            return this;
        }


        public IXClientTenantConnection ForService(string nodeHostName, int hostPort, NodeConnectionType nodeConnectionType)
        {
            if (nodeConnectionType == NodeConnectionType.NON_SSL)
                _configuration.NodeUrl = new Uri($"http://{nodeHostName}:{hostPort}");
            else
                _configuration.NodeUrl = new Uri($"https://{nodeHostName}:{hostPort}");

            return this;
        }

        public IXClientTenantConnection ForService(string nodeHostName, int hostPort, NodeConnectionType nodeConnectionType, bool isSSLCertsSkipped)
        {
            ForService(nodeHostName, hostPort, nodeConnectionType);

            if (isSSLCertsSkipped == true)
                _configuration.Settings.HttpClientHandler.ServerCertificateCustomValidationCallback +=
                       (sender, certificate, chain, sslPolicyErrors) => { return true; };

            return this;
        }

        public IXClientConfiguration WithSettings(Action<XClientSettings> settings)
        {
            settings.Invoke(_configuration.Settings);

            return this;
        }

        /// <summary>
        /// Get ClientConfiguration.
        /// </summary>
        /// <returns>Object of XClientConfiguration.</returns>
        public XClientConfiguration GetClientConfiguration()
        {
            return _configuration;
        }
    }
}
