using Andy.X.Client.Configurations;
using Andy.X.Client.Events.Consumers;
using System;
using System.Threading.Tasks;

namespace Andy.X.Client.Abstractions
{
    public abstract partial class ConsumerBase<T>
    {
        public delegate void OnMessageReceivedHandler(object sender, MessageReceivedArgs e);
        public event OnMessageReceivedHandler MessageReceived;
        private readonly XClient xClient;
        private readonly ConsumerConfiguration consumerConfiguration;

        private ConsumerNodeService consumerNodeService;
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
            consumerNodeService = new ConsumerNodeService(new ConsumerNodeProvider(xClient.GetClientConfiguration(), consumerConfiguration));
            consumerNodeService.ConsumerConnected += ConsumerNodeService_ConsumerConnected;
            consumerNodeService.ConsumerDisconnected += ConsumerNodeService_ConsumerDisconnected;
            consumerNodeService.MessageInternalReceived += ConsumerNodeService_MessageInternalReceived;

            isBuilt = true;

            return this;
        }


        public async Task SubscribeAsync()
        {
            if (isBuilt != true)
                throw new Exception("Consumer should be built before subscribing to topic");

            if (isConnected != true)
            {
                await consumerNodeService.ConnectAsync();
                isConnected = true;
            }
        }

        public async Task UnsubscribeAsync()
        {
            if (isBuilt != true)
                throw new Exception("Producer should be built before unsubscribing to topic");
            if (isConnected == true)
            {
                await consumerNodeService.DisconnectAsync();
                isConnected = false;
            }
        }

        private void ConsumerNodeService_MessageInternalReceived(MessageInternalReceivedArgs obj)
        {
            MessageReceived?.Invoke(this, new MessageReceivedArgs(obj.Id, obj.MessageRaw));
        }

        private void ConsumerNodeService_ConsumerDisconnected(ConsumerDisconnectedArgs obj)
        {
            Console.WriteLine($"andyx|{obj.Tenant}|{obj.Product}|{obj.Component}|{obj.Topic}|consumers#{obj.ConsumerName}|{obj.Id}|disconnected");

        }

        private void ConsumerNodeService_ConsumerConnected(ConsumerConnectedArgs obj)
        {
            Console.WriteLine($"andyx|{obj.Tenant}|{obj.Product}|{obj.Component}|{obj.Topic}|consumers#{obj.ConsumerName}|{obj.Id}|connected");

        }

    }
}
