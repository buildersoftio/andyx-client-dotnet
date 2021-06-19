using Andy.X.Client.Configurations;
using Andy.X.Client.Events.Consumers;
using System.Threading.Tasks;

namespace Andy.X.Client.Abstractions
{
    public abstract partial class ConsumerBase<T>
    {
        public delegate void OnMessageReceivedHandler(object sender, MessageReceivedArgs e);
        public event OnMessageReceivedHandler MessageReceived;
        private readonly XClient xClient;
        private readonly ConsumerConfiguration consumerConfiguration;

        private bool isBuilt = false;
        private bool isConnected = false;

        public ConsumerBase(XClient xClient)
        {
            this.xClient = xClient;
            consumerConfiguration = new ConsumerConfiguration();
        }
        public ConsumerBase(XClient xClient, ConsumerConfiguration consumerConfiguration)
        {
            this.xClient = xClient;
            this.consumerConfiguration = consumerConfiguration;
        }
        public ConsumerBase(IXClientFactory xClient)
        {
            this.xClient = xClient.CreateClient();
        }
        public ConsumerBase(IXClientFactory xClient, ConsumerConfiguration consumerConfiguration)
        {
            this.xClient = xClient.CreateClient();
            this.consumerConfiguration = consumerConfiguration;
        }

        public ConsumerBase<T> Component(string component)
        {
            consumerConfiguration.Component = component;
            return this;

        }

        public ConsumerBase<T> Topic(string topic)
        {
            consumerConfiguration.Topic = topic;
            return this;
        }

        public ConsumerBase<T> Name(string name)
        {
            consumerConfiguration.Name = name;
            return this;
        }

        public ConsumerBase<T> SubscriptionType(SubscriptionType subscriptionType)
        {
            consumerConfiguration.SubscriptionType = subscriptionType;
            return this;
        }

        public ConsumerBase<T> Build()
        {

            isBuilt = true;

            return this;
        }
        public async Task SubscribeAsync()
        {
            // CONNECT
        }

        public async Task UnsubscribeAsync()
        {
            // Disconnect
        }

    }
}
