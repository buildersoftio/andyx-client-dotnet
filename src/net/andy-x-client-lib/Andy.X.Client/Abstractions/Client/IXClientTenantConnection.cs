namespace Andy.X.Client.Abstractions.Client
{
    public interface IXClientTenantConnection
    {
        IXClientProductConnection AndTenant(string tenant);
        IXClientProductConnection AndTenant(string tenant, string token);
    }
}
