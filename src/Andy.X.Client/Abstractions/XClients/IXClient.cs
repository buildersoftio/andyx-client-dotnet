using Andy.X.Client.Configurations;

namespace Andy.X.Client.Abstractions.XClients
{
    public interface IXClient : IXClientServiceConnection, IXClientTenantConnection, IXClientProductConnection, IXClientConfiguration
    {
        XClientConfiguration GetClientConfiguration();
    }
}
