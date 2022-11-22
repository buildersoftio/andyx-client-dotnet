namespace Andy.X.Client.Configurations
{
    public class ConsumerConfiguration
    {
        public string Name { get; set; }
        public SubscriptionConfiguration Subscription { get; set; }
        public ComponentConfiguration Component { get; set; }
        public TopicConfiguration Topic { get; set; }

        public ConsumerSettings Settings { get; set; }

        public ConsumerConfiguration() : this("hello-world-consumer")
        {
            // it calls the other constructor
        }

        public ConsumerConfiguration(string name) : this(name, new SubscriptionConfiguration())
        {
            // it calls the other constructor
        }

        public ConsumerConfiguration(string name, SubscriptionConfiguration subscription)
        {
            Name = name;
            Subscription = subscription;

            Component = new ComponentConfiguration();
            Topic = new TopicConfiguration();

            Settings = new ConsumerSettings();
        }
    }

    public class ConsumerSettings
    {
        public CompressionType CompressionType { get; set; }

        public ConsumerSettings()
        {
            CompressionType = CompressionType.None;
        }
    }
}
