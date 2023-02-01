using Andy.X.Client.Abstractions.Consumers;
using Andy.X.Client.Abstractions.Serializers;
using Andy.X.Client.Abstractions.XClients;
using Andy.X.Client.Builders;
using Andy.X.Client.Commands;
using Andy.X.Client.Configurations;
using Andy.X.Client.Models;
using Andy.X.Client.Providers;
using Andy.X.Client.Serializers;
using Andy.X.Client.Services;
using MessagePack;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Andy.X.Client.Abstractions.Base
{
    public abstract class ConsumerBase<K, V> : IConsumer<K, V>
    {
        private Action<K, Message<V>> _clientAction;

        private readonly ILogger _logger;
        private readonly IXClient _xClient;
        private readonly ConsumerConfiguration _consumerConfiguration;

        private ConsumerNodeProvider consumerNodeProvider;
        private ConsumerNodeService consumerNodeService;

        private bool isBuilt = false;

        public ConsumerBase(IXClient xClient) : this(xClient, new ConsumerConfiguration())
        {
            // calls last constructor
        }

        public ConsumerBase(IXClientFactory xClientFactory) : this(xClientFactory.CreateClient(), new ConsumerConfiguration())
        {
            // calls last constructor
        }

        public ConsumerBase(IXClientFactory xClientFactory, ConsumerConfiguration consumerConfiguration) : this(xClientFactory.CreateClient(), consumerConfiguration)
        {
            // calls last constructor
        }

        public ConsumerBase(IXClientFactory xClientFactory, ConsumerBuilder<K, V> consumerBuilder) : this(xClientFactory.CreateClient(), consumerBuilder.ConsumerConfiguration)
        {
            // calls last constructor
        }

        public ConsumerBase(IXClient xClient, ConsumerConfiguration consumerConfiguration)
        {
            _xClient = xClient;

            _consumerConfiguration = consumerConfiguration;
            _logger = xClient
                .GetClientConfiguration().Settings.Logging
                .GetLoggerFactory()
                .CreateLogger(typeof(Consumer<K, V>));
        }


        public IConsumerConfiguration<K, V> AndSubscription(Action<SubscriptionConfiguration> config)
        {
            config.Invoke(_consumerConfiguration.Subscription);

            return this;
        }

        public IConsumerConnection<K, V> AndTopic(string topic, string description = "")
        {
            _consumerConfiguration.Topic.Name = topic;
            _consumerConfiguration.Topic.Description = description;

            return this;
        }

        public IConsumerTopicConnection<K, V> ForComponent(string component)
        {
            _consumerConfiguration.Component.Name = component;

            return this;
        }

        public IConsumerTopicConnection<K, V> ForComponent(string component, string key, string secret)
        {
            _consumerConfiguration.Component.Name = component;
            _consumerConfiguration.Component.Key = key;
            _consumerConfiguration.Component.Secret = secret;

            return this;
        }

        public IConsumerSubscriptionConfiguration<K, V> WithName(string name)
        {
            _consumerConfiguration.Name = name;

            return this;
        }

        public IConsumerConfiguration<K, V> WithSettings(Action<ConsumerSettings> settings)
        {
            settings.Invoke(_consumerConfiguration.Settings);

            return this;
        }


        public async Task SubscribeAsync()
        {
            if (isBuilt != true)
                throw new Exception("Consumer should be built before subscribing the connection");

            await consumerNodeService.ConnectAsync();
        }

        public async Task CloseAsync()
        {
            if (isBuilt != true)
                throw new Exception("Consumer should be built before closing the connection");

            await consumerNodeService.DisconnectAsync();
        }

        public Consumer<K, V> Build()
        {
            consumerNodeProvider = new ConsumerNodeProvider(_xClient.GetClientConfiguration(), _consumerConfiguration);
            consumerNodeService = new ConsumerNodeService(consumerNodeProvider, _xClient.GetClientConfiguration());

            if (_consumerConfiguration.Settings.MessageSerializer == null)
                _consumerConfiguration.Settings.AddCustomMessageSerializer(new DefaultContractlessMessageSerializer(_consumerConfiguration.Settings.CompressionType));

            consumerNodeService.ConsumerConnected += ConsumerNodeService_ConsumerConnected;
            consumerNodeService.ConsumerDisconnected += ConsumerNodeService_ConsumerDisconnected;
            consumerNodeService.AndyOrderedDisconnect += ConsumerNodeService_AndyOrderedDisconnect;

            consumerNodeService.MessageInternalReceived += ConsumerNodeService_MessageInternalReceived;

            isBuilt = true;

            return this as Consumer<K, V>;
        }

        public IConsumerMessageHandler<K, V> MessageReceivedHandler(Action<K, Message<V>> messageReceived)
        {
            _clientAction = messageReceived;

            return this;
        }

        public async Task MessageAcknowledgement(MessageAcknowledgement messageAcknowledgement, Message<V> message)
        {
            await consumerNodeService.AcknowledgeMessage(new AcknowledgeMessageArgs()
            {
                EntryId = message.EntryId,
                NodeId = message.NodeId,
                Acknowledgement = (int)messageAcknowledgement
            });
        }

        public Task AcknowledgeMessage(Message<V> message)
        {
            return MessageAcknowledgement(Commands.MessageAcknowledgement.Acknowledged, message);
        }
        public Task SkipMessage(Message<V> message)
        {
            return MessageAcknowledgement(Commands.MessageAcknowledgement.Skipped, message);
        }
        public Task UnacknowledgeMessage(Message<V> message)
        {
            return MessageAcknowledgement(Commands.MessageAcknowledgement.Unacknowledged, message);
        }

        private void ConsumerNodeService_MessageInternalReceived(Events.Consumers.MessageInternalReceivedArgs obj)
        {
            (K keyParsed, V valueParsed) = _consumerConfiguration.Settings.MessageSerializer.Deserialize<K, V>(obj.MessageId, obj.Payload);

            _clientAction?.Invoke(keyParsed,
                new Message<V>(
                    entryId: obj.EntryId,
                    nodeId: obj.NodeId,
                    headers: obj.Headers,
                    payload: valueParsed,
                    sentDate: obj.SentDate,
                    receivedDate: DateTimeOffset.UtcNow));
        }

        private async void ConsumerNodeService_AndyOrderedDisconnect(string obj)
        {
            _logger.LogError($"Consumer is disconnected with request from the server, error details: {obj}");

            await CloseAsync();
        }

        private void ConsumerNodeService_ConsumerDisconnected(Events.Consumers.ConsumerDisconnectedArgs obj)
        {
            _logger.LogWarning($"andyx-client  | Consumer '{obj.ConsumerName}|{obj.Id}' is disconnected");
        }

        private void ConsumerNodeService_ConsumerConnected(Events.Consumers.ConsumerConnectedArgs obj)
        {
            _logger.LogWarning($"andyx-client  | Consumer '{obj.ConsumerName}|{obj.Id}' is connected");
        }
    }
}
