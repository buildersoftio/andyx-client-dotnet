using Andy.X.Client.Abstractions.Consumers;
using Andy.X.Client.Builders;
using Andy.X.Client.Configurations;
using Andy.X.Client.Events.Consumers;
using Andy.X.Client.Extensions;
using MessagePack;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Andy.X.Client.Abstractions
{
    public abstract partial class ConsumerBase<T> :
        IConsumerComponentConnection<T>,
        IConsumerTopicConnection<T>,
        IConsumerNameConnection<T>,
        IConsumerInitialPositionConnection<T>,
        IConsumerSubscriptionTypeConnection<T>,
        IConsumerOtherConfiguration<T>
    {
        public delegate MessageAcknowledgement OnMessageReceivedHandler(object sender, MessageReceivedArgs<T> e);
        public event OnMessageReceivedHandler MessageReceived;

        private readonly XClient xClient;
        private readonly ConsumerConfiguration<T> consumerConfiguration;
        private readonly ILogger logger;


        private ConsumerNodeService consumerNodeService;
        private bool isBuilt = false;
        private bool isConnected = false;

        public ConsumerBase(XClient xClient) : this(xClient, new ConsumerConfiguration<T>())
        {
        }

        public ConsumerBase(IXClientFactory xClient) : this(xClient.CreateClient(), new ConsumerConfiguration<T>())
        {
        }

        public ConsumerBase(IXClientFactory xClient, ConsumerConfiguration<T> consumerConfiguration) : this(xClient.CreateClient(), consumerConfiguration)
        {
        }

        public ConsumerBase(IXClientFactory xClient, ConsumerBuilder<T> consumerBuilder) : this(xClient.CreateClient(), consumerBuilder.ConsumerConfiguration)
        {
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

        /// <summary>
        /// Subscription name
        /// </summary>
        /// <param name="subscriptionType">Subscription type</param>
        /// <returns>ConsumerBase</returns>
        public ConsumerBase<T> SubscriptionName(string subscriptionName)
        {
            consumerConfiguration.SubscriptionSettings.SubscriptionName = subscriptionName;
            return this;
        }

        /// <summary>
        /// Subscription Type represents how the Consumer consumes messages
        /// Default value SubscriptionType=Exclusive
        /// </summary>
        /// <param name="subscriptionType">Subscription type</param>
        /// <returns>ConsumerBase</returns>
        public ConsumerBase<T> SubscriptionType(SubscriptionType subscriptionType)
        {
            consumerConfiguration.SubscriptionSettings.SubscriptionType = subscriptionType;
            return this;
        }

        public ConsumerBase<T> SubscriptionMode(SubscriptionMode subscriptionMode)
        {
            consumerConfiguration.SubscriptionSettings.SubscriptionMode = subscriptionMode;
            return this;
        }

        /// <summary>
        /// Build Consumer
        /// </summary>
        /// <returns>Consumer object</returns>
        public Consumer<T> Build()
        {
            consumerNodeService = new ConsumerNodeService(new ConsumerNodeProvider(xClient.GetClientConfiguration(), consumerConfiguration), xClient.GetClientConfiguration());
            consumerNodeService.ConsumerConnected += ConsumerNodeService_ConsumerConnected;
            consumerNodeService.ConsumerDisconnected += ConsumerNodeService_ConsumerDisconnected;
            consumerNodeService.MessageInternalReceived += ConsumerNodeService_MessageInternalReceived;

            isBuilt = true;

            return this as Consumer<T>;
        }

        public async Task AcknowledgeMessage(long ledgerId, long entryId, MessageAcknowledgement messageAcknowledgement)
        {
            await consumerNodeService.AcknowledgeMessage(new AcknowledgeMessageArgs()
            {
                LedgerId = ledgerId,
                EntryId = entryId,

                Acknowledgement = (int)messageAcknowledgement
            });
        }

        private async void ConsumerNodeService_MessageInternalReceived(MessageInternalReceivedArgs obj)
        {
            T parsedPayload = MessagePackSerializer.Deserialize<T>(obj.Payload, MessagePack.Resolvers.ContractlessStandardResolver.Options);
            try
            {
                MessageAcknowledgement? isMessageAcknowledged = MessageReceived?.Invoke(this, new MessageReceivedArgs<T>(obj.Tenant,
                    obj.Product,
                    obj.Component,
                    obj.Topic,
                    obj.LedgerId,
                    obj.EntryId,
                    obj.MessageId,
                    obj.Headers,
                    obj.Payload,
                    parsedPayload,
                    obj.SentDate));

                // Ignore acknowlegment of message is topic is not persistent
                if (consumerConfiguration.IsTopicPersistent != true)
                    return;
                if (isMessageAcknowledged.HasValue)
                {
                    await consumerNodeService.AcknowledgeMessage(new AcknowledgeMessageArgs()
                    {
                        LedgerId = obj.LedgerId,
                        EntryId = obj.EntryId,
                        Acknowledgement = (int)isMessageAcknowledged.Value
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
                    LedgerId = obj.LedgerId,
                    EntryId = obj.EntryId,
                    Acknowledgement = (int)MessageAcknowledgement.Unacknowledged
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

        /// <summary>
        /// Connect to component.
        /// </summary>
        /// <param name="component">Component Name</param>
        /// <returns>Instance of ConsumerBase for Topic Configuration.</returns>
        public IConsumerTopicConnection<T> ForComponent(string component)
        {
            return ForComponent(component, "");
        }

        /// <summary>
        /// Connect to component with component token.
        /// </summary>
        /// <param name="component">Component name.</param>
        /// <param name="token">Component token</param>
        /// <returns>Instance of ConsumerBase for Topic Configuration.</returns>
        public IConsumerTopicConnection<T> ForComponent(string component, string token)
        {
            consumerConfiguration.Component = component;
            consumerConfiguration.ComponentToken = token;

            return this;
        }

        /// <summary>
        /// Connect to persistent topic.
        /// </summary>
        /// <param name="topic">Topic name</param>
        /// <returns>Instance of ConsumerBase for Name configuration.</returns>
        public IConsumerNameConnection<T> AndTopic(string topic)
        {
            return AndTopic(topic, true);
        }

        /// <summary>
        /// Connect to topic with as persistent or not.
        /// </summary>
        /// <param name="topic">Topic name.</param>
        /// <param name="isPersistent">Topic type</param>
        /// <returns>Instance of ConsumerBase for Name configuration.</returns>
        public IConsumerNameConnection<T> AndTopic(string topic, bool isPersistent)
        {
            consumerConfiguration.Topic = topic;
            consumerConfiguration.IsTopicPersistent = isPersistent;

            return this;
        }

        /// <summary>
        /// Give the name for the consumer.
        /// </summary>
        /// <example>
        /// Is recommended to call the consumer with the application name.
        /// </example>
        /// <param name="name">Name of consumer</param>
        /// <returns>Instance of ConsumerBase for InitialPosition.</returns>
        public IConsumerInitialPositionConnection<T> WithName(string name)
        {
            consumerConfiguration.Name = name;

            return this;
        }

        /// <summary>
        /// InitialPosition tells the node where to start consuming
        /// Latest - starts consuming from the moment of connection to topic,
        /// Earlest - starts consuming from the beginning.
        /// </summary>
        /// <param name="initialPosition">Initial Position</param>
        /// <returns>Instance of ConsumerBase for SubscriptionType.</returns>
        public IConsumerSubscriptionTypeConnection<T> WithInitialPosition(InitialPosition initialPosition)
        {
            consumerConfiguration.SubscriptionSettings.InitialPosition = initialPosition;

            return this;
        }

        /// <summary>
        /// Configurate consumption type for the consumer.
        /// </summary>
        /// <param name="subscriptionType">Subscription type</param>
        /// <returns>Instance of ConsumerBase.</returns>
        public IConsumerOtherConfiguration<T> AndSubscriptionType(SubscriptionType subscriptionType)
        {
            consumerConfiguration.SubscriptionSettings.SubscriptionType = subscriptionType;

            return this;
        }


        /// <summary>
        /// Start consuming messages from the topic.
        /// </summary>
        /// <returns>Task</returns>
        /// <exception cref="Exception">If consumer is not built before.</exception>
        public async Task ConnectAsync()
        {
            if (isBuilt != true)
                throw new Exception("Consumer should be built before subscribing to topic");

            if (isConnected != true)
            {
                await consumerNodeService.ConnectAsync();
                isConnected = true;
            }
        }

        /// <summary>
        /// Stop consuming of the messages from the topic.
        /// </summary>
        /// <returns>Task</returns>
        /// <exception cref="Exception">If consumer is not built before.</exception>
        public async Task DisconnectAsync()
        {
            if (isBuilt != true)
                throw new Exception("Consumer should be built before unsubscribing to topic");
            if (isConnected == true)
            {
                await consumerNodeService.DisconnectAsync();
                isConnected = false;
            }
        }
    }
}
