using Andy.X.Client.Abstractions.Client;
using Andy.X.Client.Configurations;
using Andy.X.Client.Nodes;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace Andy.X.Client
{
    public class XClient : IXClientServiceConnection, IXClientTenantConnection, IXClientProductConnection, IXClientConfiguration
    {
        private readonly XClientConfiguration _xClientConfiguration;

        /// <summary>
        /// Initialize XClient object
        /// serviceUrl default value is https://localhost:6541
        /// </summary>
        /// <param name="xClientConfiguration">xClient Configuration</param>
        private XClient(XClientConfiguration xClientConfiguration)
        {
            _xClientConfiguration = xClientConfiguration;
        }

        public static IXClientServiceConnection CreateConnection()
        {
            return new XClient(new XClientConfiguration());
        }

        public static XClient CreateConnection(XClientConfiguration xClientConfiguration)
        {
            return new XClient(xClientConfiguration);
        }


        /// <summary>
        /// Get ClientConfiguration.
        /// </summary>
        /// <returns>Object of XClientConfiguration.</returns>
        public XClientConfiguration GetClientConfiguration()
        {
            return _xClientConfiguration;
        }

        /// <summary>
        /// Add Andy X Node Service.
        /// </summary>
        /// <param name="nodeUrl">andy x node url, required to provide http or https also the port if is needed like http://localhost:6540.</param>
        /// <returns>Instance of XClient Builder for Tenant Configuration.</returns>
        public IXClientTenantConnection ForService(string nodeUrl)
        {
            _xClientConfiguration.ServiceUrl = nodeUrl;
            return this;
        }

        /// <summary>
        /// Add Andy X Node Service.
        /// </summary>
        /// <param name="nodeHostName">XNode Hostname, by default is NON_SSL Connection</param>
        /// <param name="hostPort">XNode Port</param>
        /// <returns>Instance of XClient Builder for Tenant Configuration.</returns>
        public IXClientTenantConnection ForService(string nodeHostName, int hostPort)
        {
            _xClientConfiguration.ServiceUrl = $"http://{nodeHostName}:{hostPort}";
            return this;
        }

        /// <summary>
        /// Add Andy X Node Service.
        /// </summary>
        /// <param name="nodeHostName">XNode Hostname</param>
        /// <param name="hostPort">XNode Port</param>
        /// <param name="nodeConnectionType">XNode Connection Type, can be SSL or NON_SSL Connection</param>
        /// <returns>Instance of XClient Builder for Tenant Configuration.</returns>
        public IXClientTenantConnection ForService(string nodeHostName, int hostPort, NodeConnectionType nodeConnectionType)
        {
            return ForService(nodeHostName, hostPort, nodeConnectionType, false);
        }

        /// <summary>
        /// Add Andy X Node Service.
        /// </summary>
        /// <param name="nodeHostName"> XNode Hostname</param>
        /// <param name="hostPort">XNode Port</param>
        /// <param name="nodeConnectionType">XNode Connection Type, can be SSL or NON_SSL Connection</param>
        /// <param name="isSSLCertsSkipped">This flag is applicable only if the connection is secured.</param>
        /// <returns></returns>
        public IXClientTenantConnection ForService(string nodeHostName, int hostPort, NodeConnectionType nodeConnectionType, bool isSSLCertsSkipped)
        {
            if (nodeConnectionType == NodeConnectionType.SSL)
                _xClientConfiguration.ServiceUrl = $"https://{nodeHostName}:{hostPort}";
            else
                _xClientConfiguration.ServiceUrl = $"http://{nodeHostName}:{hostPort}";

            if (isSSLCertsSkipped == true)
            {
                _xClientConfiguration.HttpClientHandler.ServerCertificateCustomValidationCallback +=
                   (sender, certificate, chain, sslPolicyErrors) => { return true; };


            }

            return this;
        }


        public IXClientProductConnection AndTenant(string tenant)
        {
            return AndTenant(tenant, "");
        }

        public IXClientProductConnection AndTenant(string tenant, string token)
        {
            _xClientConfiguration.Tenant = tenant;
            _xClientConfiguration.TenantToken = token;

            return this;
        }

        public IXClientConfiguration AndProduct(string product)
        {
            _xClientConfiguration.Product = product;
            return this;
        }

        public IXClientConfiguration ConfigLogging(ILoggerFactory loggerFactory)
        {
            _xClientConfiguration.Logging = new XClientLogging(loggerFactory);
            return this;
        }

        public IXClientConfiguration WithHttpClientHandler(Action<HttpClientHandler> httpHandler)
        {
            httpHandler.Invoke(_xClientConfiguration.HttpClientHandler);
            return this;
        }

        public XClient Build()
        {
            return this;
        }
    }
}
