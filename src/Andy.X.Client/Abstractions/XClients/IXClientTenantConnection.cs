namespace Andy.X.Client.Abstractions.XClients
{
    internal interface IXClientTenantConnection
    {
        IXClientProductConnection AndTenant(string tenant);
        IXClientProductConnection AndTenant(string tenant, string key, string secret);
    }
}
