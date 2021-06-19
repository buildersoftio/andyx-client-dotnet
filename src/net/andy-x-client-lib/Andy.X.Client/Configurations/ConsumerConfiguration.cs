namespace Andy.X.Client.Configurations
{
    public class ConsumerConfiguration
    {
        public string Component { get; set; }
        public string Topic { get; set; }
        public string Name { get; set; }

        public SubscriptionType SubscriptionType { get; set; }

        public ConsumerConfiguration()
        {
            SubscriptionType = SubscriptionType.Exclusive;
        }
    }

    public enum SubscriptionType
    {
        /// <summary>
        /// Only one reader
        /// </summary>
        Exclusive,
        /// <summary>
        /// One reader with one backup
        /// </summary>
        Failover,
        /// <summary>
        /// Shared to more than one reader.
        /// </summary>
        Shared
    }
}
