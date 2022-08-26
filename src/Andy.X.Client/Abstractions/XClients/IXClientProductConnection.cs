namespace Andy.X.Client.Abstractions.XClients
{
    internal interface IXClientProductConnection
    {
        IXClientConfiguration AndProduct(string product);
        IXClientConfiguration AndProduct(string product, string key, string secret);
    }
}
