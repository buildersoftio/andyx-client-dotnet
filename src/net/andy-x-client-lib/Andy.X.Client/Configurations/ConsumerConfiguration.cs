namespace Andy.X.Client.Configurations
{
    public class ConsumerConfiguration<T>
    {
        /// <summary>
        /// Component Token, is needed only if the node asks for it
        /// </summary>
        public string ComponentToken { get; set; }

        /// <summary>
        /// Component name where consumer will consume.
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// Topic name where consumer will consume messages
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Name is the consumer name, is mandatory field.
        /// Default value is Name=default
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If topic doesn't exists it creates topic. If IsTopicPersistent is false it will create the topic without storing the messages permanently.
        /// Default value is IsTopicPersistent=true
        /// </summary>
        public bool IsTopicPersistent { get; set; }

        public SubscriptionSettings SubscriptionSettings { get; set; }


        public ConsumerConfiguration()
        {
            SubscriptionSettings = new SubscriptionSettings();
            IsTopicPersistent = true;
            Name = "default";
        }
    }

    public class SubscriptionSettings
    {
        public string SubscriptionName { get; set; }
        /// <summary>
        /// Subscription Type represents how the Consumer consumes messages
        /// Default value SubscriptionType=Exclusive
        /// </summary>
        public SubscriptionType SubscriptionType { get; set; }

        /// <summary>
        /// InitialPosition tells the node where to start consuming
        /// Latest - starts consuming from the moment of connection to topic,
        /// Earlest - starts consuming from the bigenning.
        /// Default value is Latest
        /// </summary>
        public InitialPosition InitialPosition { get; set; }
        public SubscriptionMode SubscriptionMode { get; set; }


        public SubscriptionSettings()
        {
            SubscriptionType = SubscriptionType.Unique;
            SubscriptionMode = SubscriptionMode.Resilient;
            InitialPosition = InitialPosition.Latest;
        }
    }

    public enum SubscriptionType
    {
        /// <summary>
        /// Only one consumer
        /// </summary>
        Unique,
        /// <summary>
        /// One consumer with one backup
        /// </summary>
        Failover,
        /// <summary>
        /// Shared to more than one consumer.
        /// </summary>
        Shared
    }

    public enum SubscriptionMode
    {
        /// <summary>
        /// Durable
        /// </summary>
        Resilient,

        /// <summary>
        /// Non Durable
        /// </summary>
        NonResilient
    }
}
