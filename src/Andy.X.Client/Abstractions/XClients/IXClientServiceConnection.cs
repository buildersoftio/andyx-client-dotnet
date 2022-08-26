using Andy.X.Client.Configurations;

namespace Andy.X.Client.Abstractions.XClients
{
    internal interface IXClientServiceConnection
    {
        IXClientTenantConnection ForService(string nodeUrl);
        IXClientTenantConnection ForService(string nodeHostName, int hostPort);
        IXClientTenantConnection ForService(string nodeHostName, int hostPort, NodeConnectionType nodeConnectionType);
        IXClientTenantConnection ForService(string nodeHostName, int hostPort, NodeConnectionType nodeConnectionType, bool isSSLCertsSkipped);
    }
}
