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

namespace Andy.X.Client.Abstractions
{
    public abstract partial class ProducerBase<T>
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

        private Dictionary<string, object> defaultHeaders;

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

            defaultHeaders = new Dictionary<string, object>();
        }


        /// <summary>
        /// Component Token, is needed only if the node asks for it
        /// </summary>
        /// <param name="componentToken">Component token</param>
        /// <returns>ProducerBase</returns>
        public ProducerBase<T> ComponentToken(string componentToken)
        {
            _producerConfiguration.ComponentToken = componentToken;
            return this;
        }

        /// <summary>
        /// Component name where producer will produce.
        /// </summary>
        /// <param name="component">component name</param>
        /// <returns>ProducerBase</returns>
        public ProducerBase<T> Component(string component)
        {
            _producerConfiguration.Component = component;
            return this;
        }

        /// <summary>
        /// Topic name where producer will produce messages
        /// </summary>
        /// <param name="topic">topic name</param>
        /// <param name="isTopicPersistent">Is topic persistent flag tells the node to when it creates the topic to be persistent or not. default value is true </param>
        /// <returns>ProducerBase</returns>
        public ProducerBase<T> Topic(string topic, bool isTopicPersistent = true)
        {
            _producerConfiguration.Topic = topic;
            _producerConfiguration.IsTopicPersistent = isTopicPersistent;

            return this;
        }

        /// <summary>
        /// Name is the producer name, is mandatory field.
        /// </summary>
        /// <param name="name">Producer name</param>
        /// <returns>ProducerBase</returns>
        public ProducerBase<T> Name(string name)
        {
            _producerConfiguration.Name = name;
            return this;
        }

        /// <summary>
        /// Enable or Disable Retrying of messages producing if connection fails
        /// Default value is False
        /// </summary>
        /// <param name="isRetryProducingActive">Enable retry producing</param>
        /// <returns>ProducerBase</returns>
        public ProducerBase<T> RetryProducing(bool isRetryProducingActive)
        {
            _producerConfiguration.RetryProducing = isRetryProducingActive;
            if (isRetryProducingActive == true)
                unsentMessagesBuffer = new ConcurrentQueue<RetryTransmitMessage>();

            return this;
        }

        /// <summary>
        /// Add Headers to the Producer, these headers will be transmitted with each message
        /// </summary>
        /// <param name="key">string header key</param>
        /// <param name="value">object type value</param>
        /// <returns>this</returns>
        public ProducerBase<T> AddDefaultHeader(string key, object value)
        {
            defaultHeaders.Add(key, value);
            return this;
        }

        /// <summary>
        /// Add Headers to the Producer, these headers will be transmitted with each message
        /// </summary>
        /// <param name="headers">Keyvaluepairs headers</param>
        /// <returns></returns>
        public ProducerBase<T> AddDefaultHeader(Dictionary<string, object> headers)
        {
            foreach (var header in headers)
            {
                defaultHeaders.Add(header.Key, header.Value);
            }
            return this;
        }

        /// <summary>
        /// Tells the producer how many times to try producing the message
        /// </summary>
        /// <param name="nTimesRetry"> int number of tries</param>
        /// <returns>ProducerBase</returns>
        public ProducerBase<T> RetryProducingMessageNTimes(int nTimesRetry)
        {
            _producerConfiguration.RetryProducingMessageNTimes = nTimesRetry;
            return this;
        }

        /// <summary>
        /// Build Producer
        /// </summary>
        /// <param name="openConnection">default value is true</param>
        /// <returns>Task of ProducerBase</returns>
        public async Task<Producer<T>> BuildAsync(bool openConnection = true)
        {
            // Add default headers
            defaultHeaders.Add("andyx-client", "Andy X Client for .NET");
            defaultHeaders.Add("andyx-client-version", "v2.1.2-preview");
            defaultHeaders.Add("andyx-producer-name", _producerConfiguration.Name);
            defaultHeaders.Add("andyx-content-type", "application/json");

            producerNodeService = new ProducerNodeService(new ProducerNodeProvider(_xClient.GetClientConfiguration(), _producerConfiguration), _xClient.GetClientConfiguration());
            producerNodeService.ProducerConnected += ProducerNodeService_ProducerConnected;
            producerNodeService.ProducerDisconnected += ProducerNodeService_ProducerDisconnected;
            producerNodeService.MessageStored += ProducerNodeService_MessageStored;
            if (openConnection == true)
                await producerNodeService.ConnectAsync();

            isConnected = true;
            isBuilt = true;

            return this as Producer<T>;
        }

        /// <summary>
        /// Build Producer
        /// </summary>
        /// <param name="openConnection">default value is true</param>
        /// <returns>ProducerBase</returns>
        public Producer<T> Build(bool openConnection = true)
        {
            return BuildAsync(openConnection).Result;
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
        /// Open connection
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

        public Guid Produce(T tObject, Dictionary<string, object> headers = null)
        {
            return ProduceAsync(tObject, headers).Result;
        }

        public async Task<Guid> ProduceAsync(T tObject, Dictionary<string, object> headers = null)
        {

            headers = AddDefaultHeaderIntoMessage(headers);

            var message = new TransmitMessageArgs()
            {
                Id = Guid.NewGuid(),
                Tenant = _xClient.GetClientConfiguration().Tenant,
                Product = _xClient.GetClientConfiguration().Product,
                Component = _producerConfiguration.Component,
                Topic = _producerConfiguration.Topic,
                MessageRaw = tObject,
                Headers = headers,
                SentDate = DateTime.UtcNow,
            };

            if (producerNodeService.GetConnectionState() == HubConnectionState.Connected)
            {
                try
                {
                    await producerNodeService.TransmitMessage(message);
                }
                catch (Exception)
                {
                    EnqueueMessageToBuffer(message);
                }
            }
            else
            {
                EnqueueMessageToBuffer(message);
            }

            return message.Id;
        }

        private Dictionary<string, object> AddDefaultHeaderIntoMessage(Dictionary<string, object> headers)
        {
            if (headers == null)
                headers = new Dictionary<string, object>();

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
    }
}
