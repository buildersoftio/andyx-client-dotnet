namespace Andy.X.Client.Configurations
{
    public class ConsumerConfiguration
    {
        public string Component { get; set; }
        public bool IsTopicPersistent { get; set; }
        public string Topic { get; set; }
        public string Name { get; set; }

        public SubscriptionType SubscriptionType { get; set; }
        public InitialPosition InitialPosition { get; set; }


        public ConsumerConfiguration()
        {
            IsTopicPersistent = true;

            SubscriptionType = SubscriptionType.Exclusive;
            InitialPosition = InitialPosition.Latest;
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
