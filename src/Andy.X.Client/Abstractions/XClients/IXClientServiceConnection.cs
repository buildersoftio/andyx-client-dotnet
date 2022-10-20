using Andy.X.Client.Configurations;
using System;

namespace Andy.X.Client.Abstractions.XClients
{
    public interface IXClientServiceConnection
    {
        IXClientTenantConnection ForService(Uri nodeUrl);
        IXClientTenantConnection ForService(string nodeHostName, int hostPort, NodeConnectionType nodeConnectionType);
        IXClientTenantConnection ForService(string nodeHostName, int hostPort, NodeConnectionType nodeConnectionType, bool isSSLCertsSkipped);
    }
}
