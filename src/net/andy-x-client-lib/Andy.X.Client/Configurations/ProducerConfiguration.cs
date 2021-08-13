namespace Andy.X.Client.Configurations
{
    public class ProducerConfiguration
    {
        public string Component { get; set; }
        public string Topic { get; set; }
        public string Name { get; set; }

        // If RetryProducing is TRUE, the client need a lot of memory if there will be a lot of unsendMessages.
        public bool RetryProducing { get; set; }
        public int RetryProducingMessageNTimes { get; set; }
        public ProducerConfiguration()
        {
            RetryProducing = false;
            RetryProducingMessageNTimes = 3;
        }
    }
}
