namespace Andy.X.Client.Configurations
{
    public class XClientConfiguration
    {
        public string XNodeUrl { get; set; }
        public string Token { get; set; }

        public string Tenant { get; set; }
        public string Product { get; set; }

        public XClientConfiguration()
        {
            XNodeUrl = "https://localhost:9001";
            Tenant = "default";
            Product = "default";
        }

        public XClientConfiguration(string xNodeUrl)
        {
            XNodeUrl = xNodeUrl;
            Tenant = "default";
            Product = "default";
        }
    }
}
