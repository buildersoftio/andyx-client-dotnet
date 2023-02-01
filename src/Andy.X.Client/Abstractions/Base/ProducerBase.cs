using Andy.X.Client.Abstractions.Producers;
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
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Andy.X.Client.Abstractions.Base
{
    public abstract class ProducerBase<K, V> : IProducer<K, V>
    {
        private readonly IXClient _xClient;
        private readonly ProducerConfiguration _producerConfiguration;
        private readonly ILogger _logger;

        private readonly IDictionary<string, string> _headers;

        // Connection with Andy X Server
        private ProducerNodeService producerNodeService;
        private ProducerNodeProvider producerNodeProvider;
        private ConcurrentDictionary<Guid, MessageId> callBackResponses;

        private bool isBuilt = false;

        #region Constuctors
        public ProducerBase(IXClient xClient) : this(xClient, new ProducerConfiguration())
        {
        }

        public ProducerBase(IXClientFactory xClientFactory) : this(xClientFactory.CreateClient(), new ProducerConfiguration())
        {
        }

        public ProducerBase(IXClientFactory xClientFactory, ProducerConfiguration producerConfiguration) : this(xClientFactory.CreateClient(), producerConfiguration)
        {
        }

        public ProducerBase(IXClientFactory xClientFactory, ProducerBuilder<K, V> producerBuilder) : this(xClientFactory.CreateClient(), producerBuilder.ProducerConfiguration)
        {
        }

        public ProducerBase(IXClient xClient, ProducerConfiguration producerConfiguration)
        {
            _xClient = xClient;
            _producerConfiguration = producerConfiguration;

            _logger = _xClient.GetClientConfiguration()
                .Settings.Logging
                .GetLoggerFactory()
                .CreateLogger(typeof(Producer<K, V>));

            _headers = new Dictionary<string, string>();
        }
        #endregion
        public IProducerTopicConnection<K, V> ForComponent(string component)
        {
            _producerConfiguration.Component.Name = component;

            return this;
        }

        public IProducerTopicConnection<K, V> ForComponent(string component, string key, string secret)
        {
            _producerConfiguration.Component.Name = component;
            _producerConfiguration.Component.Key = key;
            _producerConfiguration.Component.Secret = secret;

            return this;
        }

        public IProducerConfiguration<K, V> AddDefaultHeader(string key, string value)
        {
            _headers.TryAdd(key, value);

            return this;
        }

        public IProducerConfiguration<K, V> AddDefaultHeaders(IDictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                _headers.TryAdd(header.Key, header.Value);
            }

            return this;
        }

        public IProducerConnection<K, V> AndTopic(string topic, string description = "")
        {
            _producerConfiguration.Topic.Name = topic;
            _producerConfiguration.Topic.Description = description;

            return this;
        }

        public Producer<K, V> Build()
        {

            if (_producerConfiguration.Settings.RequireCallback == true)
                callBackResponses = new ConcurrentDictionary<Guid, MessageId>();

            if (_producerConfiguration.Settings.MessageSerializer == null)
                _producerConfiguration.Settings.AddCustomMessageSerializer(new DefaultContractlessMessageSerializer(_producerConfiguration.Settings.CompressionType));

            _headers.Add("andyx-client", "Andy X Client for .NET");
            _headers.Add("andyx-client-version", "v3.1.0");
            _headers.Add("andyx-producer-name", _producerConfiguration.Name);
            _headers.Add("andyx-content-type", "application/andyxbinary+json");


            producerNodeProvider = new ProducerNodeProvider(_xClient.GetClientConfiguration(), _producerConfiguration);
            producerNodeService = new ProducerNodeService(producerNodeProvider, _xClient.GetClientConfiguration());

            producerNodeService.ProducerConnected += ProducerNodeService_ProducerConnected;
            producerNodeService.ProducerDisconnected += ProducerNodeService_ProducerDisconnected;
            producerNodeService.MessageAccepted += ProducerNodeService_MessageAccepted;
            producerNodeService.AndyOrderedDisconnect += ProducerNodeService_AndyOrderedDisconnect;

            isBuilt = true;

            return this as Producer<K, V>;
        }

        private async void ProducerNodeService_AndyOrderedDisconnect(string obj)
        {
            _logger.LogError($"Producer is disconnected with request from the server, error details: {obj}"); await CloseAsync();
        }

        private void ProducerNodeService_MessageAccepted(Events.Producers.MessageAcceptedArgs obj)
        {
            callBackResponses
                .TryAdd(obj.IdentityId, new MessageId(obj.IdentityId, obj.MessageCount, obj.AcceptedDate));
        }

        private void ProducerNodeService_ProducerDisconnected(Events.Producers.ProducerDisconnectedArgs obj)
        {
            _logger.LogWarning($"andyx-client  | Producer '{obj.ProducerName}|{obj.Id}' is disconnected");
        }

        private void ProducerNodeService_ProducerConnected(Events.Producers.ProducerConnectedArgs obj)
        {
            _logger.LogWarning($"andyx-client  | Producer '{obj.ProducerName}|{obj.Id}' is connected");
        }

        public async Task CloseAsync()
        {
            if (isBuilt != true)
                throw new Exception("Producer should be built before closing the connection");

            await producerNodeService.DisconnectAsync();
        }


        public async Task OpenAsync()
        {
            if (isBuilt != true)
                throw new Exception("Producer should be built before connecting the connection");

            await producerNodeService.ConnectAsync();
        }

        public async Task<MessageId> SendAsync(K id, V message, IDictionary<string, string> headers = null, string sendNodeId = "")
        {
            headers = TryAddHeaders(headers);

            var identityId = Guid.NewGuid();
            var result = new MessageId(identityId, -1, DateTimeOffset.UtcNow);

            if (producerNodeService.GetConnectionState() == HubConnectionState.Connected)
            {
                TransmitMessageArgs transmitMessage = CreateTransmitMessage(id, message, headers, sendNodeId, identityId);
                await producerNodeService.TransmitMessage(transmitMessage);
            }
            else
            {
                if (_producerConfiguration.Settings.BreakIfTryToSendMessageInClosedConneciton == true)
                    throw new Exception($"Connection with Andy X is not establised, message is not produced, connectionState: {producerNodeService.GetConnectionState()}. To ignore and not to throw an exception, update Producer Settings 'BreakIfTryToSendMessageInClosedConneciton' to false");
            }

            return CheckForCallbackResponse(identityId, result);
        }

        public async Task<MessageId> SendAsync(IDictionary<K, V> messages, IDictionary<string, string> headers = null, string sendNodeId = "")
        {
            headers = TryAddHeaders(headers);
            var messagesArgs = new List<TransmitMessageArgs>();

            var identityId = Guid.NewGuid();
            var result = new MessageId(identityId, -1, DateTimeOffset.UtcNow);


            if (producerNodeService.GetConnectionState() == HubConnectionState.Connected)
            {
                foreach (var message in messages)
                {
                    var transmitMessage = CreateTransmitMessage(message.Key, message.Value, headers, sendNodeId, identityId);
                    messagesArgs.Add(transmitMessage);
                }

                await producerNodeService.TransmitMessages(messagesArgs);
            }
            else
            {
                if (_producerConfiguration.Settings.BreakIfTryToSendMessageInClosedConneciton == true)
                    throw new Exception($"Connection with Andy X is not establised, messages are not produced, connectionState: {producerNodeService.GetConnectionState()}. To ignore and not to throw an exception, update Producer Settings 'BreakIfTryToSendMessageInClosedConneciton' to false");
            }

            return CheckForCallbackResponse(identityId, result);
        }

        public IProducerConfiguration<K, V> WithName(string name)
        {
            _producerConfiguration.Name = name;

            return this;
        }

        public IProducerConfiguration<K, V> WithSettings(Action<ProducerSettings> settings)
        {
            settings.Invoke(_producerConfiguration.Settings);

            return this;
        }

        private IDictionary<string, string> TryAddHeaders(IDictionary<string, string> headers)
        {
            if (headers == null)
            {
                return _headers;
            }

            foreach (var defaultHeader in _headers)
            {
                headers.Add(defaultHeader.Key, defaultHeader.Value);
            }

            return headers;
        }

        private MessageId CheckForCallbackResponse(Guid identityId, MessageId result)
        {
            if (_producerConfiguration.Settings.RequireCallback == true)
            {
                int milisecondsWait = 0;
                int sleepTimeMilisec = 1;
                while (callBackResponses.ContainsKey(identityId) != true)
                {
                    Thread.Sleep(sleepTimeMilisec);
                    milisecondsWait += sleepTimeMilisec;
                    if (milisecondsWait >= _producerConfiguration.Settings.TimeoutInSyncResponseMs)
                        throw new Exception($"Run in timeout, couldnot get response from Andy X. TimeoutInSyncResponseMs: {_producerConfiguration.Settings.TimeoutInSyncResponseMs}. You can increase TimeoutInSyncResponseMs at Producer Settings.");

                }
                callBackResponses.TryRemove(identityId, out result);
            }

            return result;
        }


        private TransmitMessageArgs CreateTransmitMessage(K id, V message, IDictionary<string, string> headers, string sendNodeId, Guid identityId)
        {
            var transmitMessage = new TransmitMessageArgs()
            {
                Tenant = _xClient.GetClientConfiguration().Tenant.Name,
                Product = _xClient.GetClientConfiguration().Product.Name,
                Component = _producerConfiguration.Component.Name,
                Topic = _producerConfiguration.Topic.Name,

                IdentityId = identityId,
                Headers = headers as Dictionary<string, string>,

                SentDate = DateTimeOffset.UtcNow,

                NodeId = sendNodeId,
                RequiresCallback = _producerConfiguration.Settings.RequireCallback
            };

            (transmitMessage.Id, transmitMessage.Payload) = _producerConfiguration.Settings.MessageSerializer.Serialize(id, message);
            return transmitMessage;
        }

    }
}
