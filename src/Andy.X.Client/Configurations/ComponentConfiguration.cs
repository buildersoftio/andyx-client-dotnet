namespace Andy.X.Client.Configurations
{
    public class ComponentConfiguration
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }

        public ComponentConfiguration()
        {
            Name = "default";
        }
    }
}
