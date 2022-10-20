namespace Andy.X.Client.Abstractions.XClients
{
    public interface IXClientTenantConnection
    {
        IXClientProductConnection AndTenant(string tenant);
        IXClientProductConnection AndTenant(string tenant, string key, string secret);
    }
}
