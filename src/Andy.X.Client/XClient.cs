using Andy.X.Client.Abstractions.XClients;
using Andy.X.Client.Configurations;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace Andy.X.Client
{
    public class XClient : IXClientServiceConnection, IXClientTenantConnection, IXClientProductConnection, IXClientConfiguration
    {
        private XClient()
        {

        }

        public static IXClientTenantConnection CreateClient()
        {
            return new XClient();
        }

        public XClient Build()
        {
            throw new NotImplementedException();
        }

        IXClientConfiguration IXClientConfiguration.AddLoggingSupport(ILoggerFactory loggerFactory)
        {
            throw new NotImplementedException();
        }

        IXClientConfiguration IXClientProductConnection.AndProduct(string product)
        {
            throw new NotImplementedException();
        }

        IXClientConfiguration IXClientProductConnection.AndProduct(string product, string key, string secret)
        {
            throw new NotImplementedException();
        }

        IXClientProductConnection IXClientTenantConnection.AndTenant(string tenant)
        {
            throw new NotImplementedException();
        }

        IXClientProductConnection IXClientTenantConnection.AndTenant(string tenant, string key, string secret)
        {
            throw new NotImplementedException();
        }

        IXClientTenantConnection IXClientServiceConnection.ForService(string nodeUrl)
        {
            throw new NotImplementedException();
        }

        IXClientTenantConnection IXClientServiceConnection.ForService(string nodeHostName, int hostPort)
        {
            throw new NotImplementedException();
        }

        IXClientTenantConnection IXClientServiceConnection.ForService(string nodeHostName, int hostPort, NodeConnectionType nodeConnectionType)
        {
            throw new NotImplementedException();
        }

        IXClientTenantConnection IXClientServiceConnection.ForService(string nodeHostName, int hostPort, NodeConnectionType nodeConnectionType, bool isSSLCertsSkipped)
        {
            throw new NotImplementedException();
        }

        IXClientConfiguration IXClientConfiguration.WithHttpClientHandler(Action<HttpClientHandler> httpHandler)
        {
            throw new NotImplementedException();
        }
    }
}
