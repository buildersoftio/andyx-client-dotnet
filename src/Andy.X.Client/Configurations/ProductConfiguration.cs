namespace Andy.X.Client.Configurations
{
    public class ProductConfiguration
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public ProductConfiguration()
        {
            Name = "public";
        }
    }
}
