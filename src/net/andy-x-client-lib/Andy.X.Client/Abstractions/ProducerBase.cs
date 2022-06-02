using Andy.X.Client.Configurations;
using Andy.X.Client.Events.Producers;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Andy.X.Client.Builders;
using Andy.X.Client.Abstractions.Producers;
using Andy.X.Client.Extensions;
using System.Text;
using MessagePack;

namespace Andy.X.Client.Abstractions
{
    public abstract partial class ProducerBase<T> :
        IProducerComponentConnection<T>,
        IProducerTopicConnection<T>,
        IProducerNameConnection<T>,
        IProducerOtherConfiguration<T>
    {
        private readonly XClient _xClient;
        private readonly ProducerConfiguration<T> _producerConfiguration;
        private readonly ILogger _logger;

        public delegate void OnMessageStoredHandler(object sender, MessageStoredArgs e);
        public event OnMessageStoredHandler MessageStored;

        private ProducerNodeService producerNodeService;
        private bool isBuilt = false;
        private bool isConnected = false;

        private ConcurrentQueue<RetryTransmitMessage> unsentMessagesBuffer;
        private bool isUnsentMessagesProcessorWorking = false;

        private Dictionary<string, string> defaultHeaders;

        public ProducerBase(XClient xClient) : this(xClient, new ProducerConfiguration<T>())
        {

        }

        public ProducerBase(IXClientFactory xClient) : this(xClient.CreateClient(), new ProducerConfiguration<T>())
        {

        }
        public ProducerBase(IXClientFactory xClient, ProducerConfiguration<T> producerConfiguration) : this(xClient.CreateClient(), producerConfiguration)
        {

        }

        public ProducerBase(IXClientFactory xClient, ProducerBuilder<T> producerBuilder) : this(xClient.CreateClient(), producerBuilder.ProducerConfiguration)
        {

        }

        public ProducerBase(XClient xClient, ProducerConfiguration<T> producerConfiguration)
        {
            _xClient = xClient;
            _producerConfiguration = producerConfiguration;

            _logger = _xClient.GetClientConfiguration()
                .Logging
                .GetLoggerFactory()
                .CreateLogger(typeof(T));

            if (producerConfiguration.RetryProducing == true)
                unsentMessagesBuffer = new ConcurrentQueue<RetryTransmitMessage>();

            defaultHeaders = new Dictionary<string, string>();
        }

        /// <summary>
        /// Close connection
        /// </summary>
        /// <returns>Task</returns>
        /// <exception cref="Exception">Producer should be built before closing</exception>
        public async Task CloseAsync()
        {
            if (isBuilt != true)
                throw new Exception("Producer should be built before closing the connection");
            if (isConnected == true)
            {
                await producerNodeService.DisconnectAsync();
                isConnected = false;
            }
        }

        /// <summary>
        /// Open connection and start producer
        /// </summary>
        /// <returns>Task</returns>
        /// <exception cref="Exception">Producer should be built before closing</exception>

        public async Task OpenAsync()
        {
            if (isBuilt != true)
                throw new Exception("Producer should be built before connecting  the connection");

            if (isConnected != true)
            {
                await producerNodeService.ConnectAsync();
                isConnected = true;
            }
        }


        private void ProducerNodeService_MessageStored(MessageStoredArgs obj)
        {
            MessageStored?.Invoke(this, obj);
        }

        private void ProducerNodeService_ProducerDisconnected(ProducerDisconnectedArgs obj)
        {
            _logger.LogWarning($"andyx-client  | Producer '{obj.ProducerName}|{obj.Id}' is disconnected");
        }

        private void ProducerNodeService_ProducerConnected(ProducerConnectedArgs obj)
        {
            _logger.LogWarning($"andyx-client  | Producer '{obj.ProducerName}|{obj.Id}' is connected");
        }

        public Guid Produce(T tObject, Dictionary<string, string> headers = null)
        {
            return ProduceAsync(tObject, headers).Result;
        }

        public List<Guid> Produce(IList<T> messages, Dictionary<string, string> headers = null)
        {
            return ProduceAsync(messages, headers).Result;
        }

        public async Task<List<Guid>> ProduceAsync(IList<T> messages, Dictionary<string, string> headers = null)
        {
            headers = AddDefaultHeaderIntoMessage(headers);
            var ids = new List<Guid>();
            var messagesArgs = new List<TransmitMessageArgs>();
            foreach (var msg in messages)
            {
                Guid id = Guid.NewGuid();
                // MessagePackSerializer.Serialize(msg);
                messagesArgs.Add(new TransmitMessageArgs()
                {
                    Id = id.ToString(),
                    Tenant = _xClient.GetClientConfiguration().Tenant,
                    Product = _xClient.GetClientConfiguration().Product,
                    Component = _producerConfiguration.Component,
                    Topic = _producerConfiguration.Topic,
                    // this one works too, but for now we will use ContractlessStandardResolver. When we will create the schema registry we will enable resolvers.
                    //Payload = MessagePackSerializer.Typeless.Serialize(msg),
                    Payload = MessagePackSerializer.Serialize(msg, MessagePack.Resolvers.ContractlessStandardResolver.Options),
                    Headers = headers,
                    SentDate = DateTimeOffset.UtcNow,
                });
                ids.Add(id);
            }

            if (producerNodeService.GetConnectionState() == HubConnectionState.Connected)
            {
                try
                {
                    await producerNodeService.TransmitMessages(messagesArgs);
                    return ids;
                }
                catch (Exception)
                {
                    // moved the code to the end of method.
                }
            }

            // Messages are not send
            messagesArgs.ForEach(x =>
            {
                EnqueueMessageToBuffer(x);
            });

            return new List<Guid>();
        }

        public async Task<Guid> ProduceAsync(T message, Dictionary<string, string> headers = null)
        {
            headers = AddDefaultHeaderIntoMessage(headers);

            var messageArgs = new TransmitMessageArgs()
            {
                Id = Guid.NewGuid().ToString(),
                Tenant = _xClient.GetClientConfiguration().Tenant,
                Product = _xClient.GetClientConfiguration().Product,
                Component = _producerConfiguration.Component,
                Topic = _producerConfiguration.Topic,
                // this one works too, but for now we will use ContractlessStandardResolver. When we will create the schema registry we will enable resolvers.
                //Payload = MessagePackSerializer.Typeless.Serialize(message),
                Payload = MessagePackSerializer.Serialize(message, MessagePack.Resolvers.ContractlessStandardResolver.Options),
                Headers = headers,
                SentDate = DateTimeOffset.UtcNow,
            };

            if (producerNodeService.GetConnectionState() == HubConnectionState.Connected)
            {
                try
                {
                    await producerNodeService.TransmitMessage(messageArgs);
                    return Guid.Parse(messageArgs.Id);
                }
                catch (Exception)
                {
                    // moved the code to the end of method.
                }
            }

            EnqueueMessageToBuffer(messageArgs);
            return Guid.Empty;
        }

        private Dictionary<string, string> AddDefaultHeaderIntoMessage(Dictionary<string, string> headers)
        {
            if (headers == null)
                headers = new Dictionary<string, string>();

            foreach (var defaultHeader in defaultHeaders)
            {
                headers.Add(defaultHeader.Key, defaultHeader.Value);
            }
            return headers;
        }

        private void EnqueueMessageToBuffer(TransmitMessageArgs message)
        {
            _logger.LogWarning($"andyx-client  | Producing of message '{message.Id}' at {message.Tenant}/{message.Product}/{message.Component}/{message.Topic} failed, retrying 1 of {_producerConfiguration.RetryProducingMessageNTimes} tires");
            if (_producerConfiguration.RetryProducing == true)
            {
                unsentMessagesBuffer.Enqueue(new RetryTransmitMessage()
                {
                    TransmitMessageArgs = message,
                    RetryCounter = 1
                });

                InitializeUnsentMessageProcessor();
            }
        }

        #region UnsentMessageProcessor
        private void InitializeUnsentMessageProcessor()
        {
            if (isUnsentMessagesProcessorWorking != true)
            {
                isUnsentMessagesProcessorWorking = true;

                new Thread(() => UnsentMessageProcessor()).Start();
            }
        }

        private async void UnsentMessageProcessor()
        {
            while (unsentMessagesBuffer.IsEmpty != true)
            {
                RetryTransmitMessage retryTransmitMessage;
                bool isMessageReturned = unsentMessagesBuffer.TryDequeue(out retryTransmitMessage);
                if (isMessageReturned == true)
                {
                    if (retryTransmitMessage.RetryCounter < _producerConfiguration.RetryProducingMessageNTimes)
                    {
                        retryTransmitMessage.RetryCounter++;

                        if (producerNodeService.GetConnectionState() == HubConnectionState.Connected)
                        {
                            try
                            {
                                await producerNodeService.TransmitMessage(retryTransmitMessage.TransmitMessageArgs);
                            }
                            catch (Exception)
                            {
                                unsentMessagesBuffer.Enqueue(retryTransmitMessage);
                            }
                        }
                        else
                            unsentMessagesBuffer.Enqueue(retryTransmitMessage);
                    }
                    else
                    {
                        // If RetryCounter is bigger than RetryProducerMessageNTimes ignore that message.
                        _logger.LogError($"andyx-client  | Producing of message '{retryTransmitMessage.TransmitMessageArgs.Id}' " +
                            $"at {retryTransmitMessage.TransmitMessageArgs.Tenant}/{retryTransmitMessage.TransmitMessageArgs.Product}" +
                            $"/{retryTransmitMessage.TransmitMessageArgs.Component}/{retryTransmitMessage.TransmitMessageArgs.Topic} failed, message is lost");
                    }
                }
            }
            isUnsentMessagesProcessorWorking = false;
        }
        #endregion

        /// <summary>
        /// Connect to component.
        /// </summary>
        /// <param name="component">Component Name</param>
        /// <returns>Instance of ProducerBase for Topic Configuration.</returns>
        public IProducerTopicConnection<T> ForComponent(string component)
        {
            ForComponent(component, "");

            return this;
        }

        /// <summary>
        /// Connect to component with component token.
        /// </summary>
        /// <param name="component">Component name.</param>
        /// <param name="token">Component token</param>
        /// <returns>Instance of ProducerBase for Topic Configuration.</returns>
        public IProducerTopicConnection<T> ForComponent(string component, string token)
        {
            _producerConfiguration.Component = component;
            _producerConfiguration.ComponentToken = token;

            return this;
        }

        /// <summary>
        /// Connect to persistent topic.
        /// </summary>
        /// <param name="topic">Topic name</param>
        /// <returns>Instance of ProducerBase for Name configuration.</returns>
        public IProducerNameConnection<T> AndTopic(string topic)
        {
            return AndTopic(topic, true);
        }

        /// <summary>
        /// Connect to topic with as persistent or not.
        /// </summary>
        /// <param name="topic">Topic name.</param>
        /// <param name="isPersistent">Topic type</param>
        /// <returns>Instance of ProducerBase for Name configuration.</returns>
        public IProducerNameConnection<T> AndTopic(string topic, bool isPersistent)
        {
            _producerConfiguration.Topic = topic;
            _producerConfiguration.IsTopicPersistent = isPersistent;

            return this;
        }

        /// <summary>
        /// Give the name for the producer.
        /// </summary>
        /// <example>
        /// Is recommended to call the producer with the application name.
        /// </example>
        /// <param name="name">Name of producer</param>
        /// <returns>Instance of ProducerBase.</returns>
        public IProducerOtherConfiguration<T> WithName(string name)
        {
            _producerConfiguration.Name = name;
            return this;
        }

        /// <summary>
        /// Add Headers to the message.
        /// </summary>
        /// <param name="key">key string value</param>
        /// <param name="value">value object for the header</param>
        /// <returns>Instance of ProducerBase.</returns>
        public IProducerOtherConfiguration<T> AddDefaultHeader(string key, string value)
        {
            defaultHeaders.Add(key, value);
            return this;
        }

        /// <summary>
        /// Add Headers to the message.
        /// </summary>
        /// <param name="headers">Dictionary object for the headers</param>
        /// <returns>Instance of ProducerBase.</returns>
        public IProducerOtherConfiguration<T> AddDefaultHeader(IDictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                defaultHeaders.Add(header.Key, header.Value);
            }
            return this;
        }

        /// <summary>
        /// Enable retrying of message producing, if production of messages fails.
        /// </summary>
        /// <returns>Instance of ProducerBase.</returns>
        public IProducerOtherConfiguration<T> RetryProducingIfFails()
        {
            _producerConfiguration.RetryProducing = true;
            unsentMessagesBuffer = new ConcurrentQueue<RetryTransmitMessage>();
            return this;
        }

        /// <summary>
        /// Configure the number of tries before droping the messages if connection fails.
        /// </summary>
        /// <param name="nTimesRetry">Max number of tries, type int</param>
        /// <returns>Instance of ProducerBase</returns>
        public IProducerOtherConfiguration<T> HowManyTimesToTryProducing(int nTimesRetry)
        {
            _producerConfiguration.RetryProducingMessageNTimes = nTimesRetry;

            return this;
        }

        /// <summary>
        /// Build Producer
        /// </summary>
        /// <returns>Producer object</returns>
        public Producer<T> Build()
        {
            // Add default headers
            defaultHeaders.Add("andyx-client", "Andy X Client for .NET");
            defaultHeaders.Add("andyx-client-version", "v3.0.0-alpha1");
            defaultHeaders.Add("andyx-producer-name", _producerConfiguration.Name);
            defaultHeaders.Add("andyx-content-type", "application/json");

            producerNodeService = new ProducerNodeService(new ProducerNodeProvider(_xClient.GetClientConfiguration(), _producerConfiguration), _xClient.GetClientConfiguration());
            producerNodeService.ProducerConnected += ProducerNodeService_ProducerConnected;
            producerNodeService.ProducerDisconnected += ProducerNodeService_ProducerDisconnected;
            producerNodeService.MessageStored += ProducerNodeService_MessageStored;

            isConnected = false;
            isBuilt = true;

            return this as Producer<T>;
        }
    }
}
