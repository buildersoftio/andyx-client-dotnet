using Andy.X.Client.Configurations;
using Andy.X.Client.Events.Consumers;
using Andy.X.Client.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Andy.X.Client.Abstractions
{
    public abstract partial class ConsumerBase<T>
    {
        public delegate bool OnMessageReceivedHandler(object sender, MessageReceivedArgs<T> e);
        public event OnMessageReceivedHandler MessageReceived;

        private readonly XClient xClient;
        private readonly ConsumerConfiguration<T> consumerConfiguration;
        private readonly ILogger logger;


        private ConsumerNodeService consumerNodeService;
        private bool isBuilt = false;
        private bool isConnected = false;

        public ConsumerBase(XClient xClient)
        {
            this.xClient = xClient;
            consumerConfiguration = new ConsumerConfiguration<T>();
            logger = this.xClient.GetClientConfiguration()
                .Logging
                .GetLoggerFactory()
                .CreateLogger(typeof(T));
        }
        public ConsumerBase(XClient xClient, ConsumerConfiguration<T> consumerConfiguration)
        {
            this.xClient = xClient;
            this.consumerConfiguration = consumerConfiguration;
            logger = this.xClient.GetClientConfiguration()
                .Logging
                .GetLoggerFactory()
                .CreateLogger(typeof(T));
        }
        public ConsumerBase(IXClientFactory xClient)
        {
            this.xClient = xClient.CreateClient();

            logger = this.xClient.GetClientConfiguration()
                .Logging
                .GetLoggerFactory()
                .CreateLogger(typeof(T));
        }
        public ConsumerBase(IXClientFactory xClient, ConsumerConfiguration<T> consumerConfiguration)
        {
            this.xClient = xClient.CreateClient();
            this.consumerConfiguration = consumerConfiguration;

            logger = this.xClient.GetClientConfiguration()
                .Logging
                .GetLoggerFactory()
                .CreateLogger(typeof(T));
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

        public ConsumerBase<T> InitialPosition(InitialPosition initialPosition)
        {
            consumerConfiguration.InitialPosition = initialPosition;
            return this;
        }

        public ConsumerBase<T> Build()
        {
            consumerNodeService = new ConsumerNodeService(new ConsumerNodeProvider(xClient.GetClientConfiguration(), consumerConfiguration), xClient.GetClientConfiguration());
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
                throw new Exception("Consumer should be built before unsubscribing to topic");
            if (isConnected == true)
            {
                await consumerNodeService.DisconnectAsync();
                isConnected = false;
            }
        }

        private async void ConsumerNodeService_MessageInternalReceived(MessageInternalReceivedArgs obj)
        {
            T parsedPayload = obj.MessageRaw.ToJson().TryJsonToObject<T>();
            try
            {
                bool? isMessageAcknowledged = MessageReceived?.Invoke(this, new MessageReceivedArgs<T>(obj.Tenant, obj.Product, obj.Component, obj.Topic, obj.Id, obj.MessageRaw, parsedPayload, obj.SentDate));

                // Ignore acknowlegment of message is topic is not persistent
                if (consumerConfiguration.IsTopicPersistent != true)
                    return;
                if (isMessageAcknowledged.HasValue)
                {
                    await consumerNodeService.AcknowledgeMessage(new AcknowledgeMessageArgs()
                    {
                        Tenant = obj.Tenant,
                        Product = obj.Product,
                        Component = obj.Component,
                        Topic = obj.Topic,
                        Consumer = consumerConfiguration.Name,
                        IsAcknowledged = isMessageAcknowledged.Value,
                        MessageId = obj.Id
                    });
                }
            }
            catch (Exception ex)
            {
                // ignore acknowlegment of message is topic is not persistent
                if (consumerConfiguration.IsTopicPersistent != true)
                    return;

                await consumerNodeService.AcknowledgeMessage(new AcknowledgeMessageArgs()
                {
                    Tenant = obj.Tenant,
                    Product = obj.Product,
                    Component = obj.Component,
                    Topic = obj.Topic,
                    Consumer = consumerConfiguration.Name,
                    IsAcknowledged = false,
                    MessageId = obj.Id
                });
                logger.LogError($"MessageReceived failed to process, message is not acknowledged. Error description: '{ex.Message}'");
            }
        }

        private void ConsumerNodeService_ConsumerDisconnected(ConsumerDisconnectedArgs obj)
        {
            logger.LogWarning($"andyx-client  | Consumer '{obj.ConsumerName}|{obj.Id}' is disconnected");
        }

        private void ConsumerNodeService_ConsumerConnected(ConsumerConnectedArgs obj)
        {
            logger.LogWarning($"andyx-client  | Consumer '{obj.ConsumerName}|{obj.Id}' is connected");

        }
    }
}
