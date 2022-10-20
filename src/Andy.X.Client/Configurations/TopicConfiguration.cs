namespace Andy.X.Client.Configurations
{
    public class TopicConfiguration
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public TopicConfiguration()
        {
            Name = "hello-world";
            Description = "hello-world topic created from Andy X Client for .NET";
        }
    }
}
