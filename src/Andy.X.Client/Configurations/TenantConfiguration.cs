namespace Andy.X.Client.Configurations
{
    public class TenantConfiguration
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }

        public TenantConfiguration()
        {
            Name = "default";
        }
    }

}
