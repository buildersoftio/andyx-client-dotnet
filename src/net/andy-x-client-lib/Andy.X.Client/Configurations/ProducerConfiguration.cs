namespace Andy.X.Client.Configurations
{
    public class ProducerConfiguration<T>
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
        /// If topic doesn't exists it creates topic. If IsTopicPersistent is false it will create the topic without storing the messages permanently.
        /// Default value is IsTopicPersistent=true
        /// </summary>
        public bool IsTopicPersistent { get; set; }

        /// <summary>
        /// Topic name where consumer will consume messages
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// Name is the producer name, is mandatory field.
        /// Default value is Name=default
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If RetryProducing is TRUE, the client need a lot of memory if there will be a lot of unsendMessages
        /// </summary>
        public bool RetryProducing { get; set; }

        /// <summary>
        /// Configure how many times should produce tries to produce undelivered messages.
        /// </summary>
        public int RetryProducingMessageNTimes { get; set; }

        public ProducerConfiguration()
        {
            RetryProducing = false;
            RetryProducingMessageNTimes = 3;
            IsTopicPersistent = true;
        }
    }
}
