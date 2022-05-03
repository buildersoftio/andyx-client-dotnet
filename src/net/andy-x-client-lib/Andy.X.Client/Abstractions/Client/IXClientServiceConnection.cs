using Andy.X.Client.Nodes;

namespace Andy.X.Client.Abstractions.Client
{
    public interface IXClientServiceConnection
    {
       IXClientTenantConnection ForService(string nodeUrl);
       IXClientTenantConnection ForService(string nodeHostName, int hostPort);
       IXClientTenantConnection ForService(string nodeHostName, int hostPort, NodeConnectionType nodeConnectionType);
       IXClientTenantConnection ForService(string nodeHostName, int hostPort, NodeConnectionType nodeConnectionType, bool isSSLCertsSkipped);
    }
}
