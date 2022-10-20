namespace Andy.X.Client.Configurations
{
    public class SubscriptionConfiguration
    {
        public string Name { get; set; }

        public SubscriptionType Type { get; set; }
        public SubscriptionMode Mode { get; set; }
        public InitialPosition InitialPosition { get; set; }

        public SubscriptionConfiguration(): this("hello-world-subscription")
        {

        }

        public SubscriptionConfiguration(string name)
        {
            Name = name;

            Type = SubscriptionType.Unique;
            Mode = SubscriptionMode.Resilient;
            InitialPosition = InitialPosition.Latest;
        }
    }

    public enum SubscriptionType
    {
        /// <summary>
        /// Only one consumer instance
        /// </summary>
        Unique,
        /// <summary>
        /// One consumer instance with other backup
        /// </summary>
        Failover,
        /// <summary>
        /// Shared to more than one consumer instance.
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

    public enum InitialPosition
    {
        /// <summary>
        /// Register pointer of entry to the earliest message entry position in topic
        /// </summary>
        Earliest,

        /// <summary>
        /// Register pointer of entry to the lastest message entry position
        /// </summary>
        Latest,

        /// <summary>
        /// Register pointer of entry to some time ago message entry position. Version 3.0 will not support InTime
        /// </summary>
        //InTime
    }
}
