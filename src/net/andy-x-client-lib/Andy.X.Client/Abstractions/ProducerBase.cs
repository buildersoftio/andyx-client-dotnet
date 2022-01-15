using Andy.X.Client.Configurations;
using Andy.X.Client.Events.Producers;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Andy.X.Client.Abstractions
{
    public abstract partial class ProducerBase<T>
    {
        private readonly XClient xClient;
        private readonly ProducerConfiguration<T> producerConfiguration;
        private readonly ILogger logger;

        public delegate void OnMessageStoredHandler(object sender, MessageStoredArgs e);
        public event OnMessageStoredHandler MessageStored;

        private ProducerNodeService producerNodeService;
        private bool isBuilt = false;
        private bool isConnected = false;

        private ConcurrentQueue<RetryTransmitMessage> unsentMessagesBuffer;
        private bool isUnsentMessagesProcessorWorking = false;

        public ProducerBase(XClient xClient)
        {
            this.xClient = xClient;
            producerConfiguration = new ProducerConfiguration<T>();

            logger = this.xClient.GetClientConfiguration()
                .Logging
                .GetLoggerFactory()
                .CreateLogger(typeof(T));
        }
        public ProducerBase(XClient xClient, ProducerConfiguration<T> producerConfiguration)
        {
            this.xClient = xClient;
            this.producerConfiguration = producerConfiguration;

            logger = this.xClient.GetClientConfiguration()
                .Logging
                .GetLoggerFactory()
                .CreateLogger(typeof(T));

            if (producerConfiguration.RetryProducing == true)
                unsentMessagesBuffer = new ConcurrentQueue<RetryTransmitMessage>();
        }
        public ProducerBase(IXClientFactory xClient)
        {
            this.xClient = xClient.CreateClient();
            producerConfiguration = new ProducerConfiguration<T>();

            logger = this.xClient.GetClientConfiguration()
                .Logging
                .GetLoggerFactory()
                .CreateLogger(typeof(T));

        }
        public ProducerBase(IXClientFactory xClient, ProducerConfiguration<T> producerConfiguration)
        {
            this.xClient = xClient.CreateClient();
            this.producerConfiguration = producerConfiguration;

            logger = this.xClient.GetClientConfiguration()
                .Logging
                .GetLoggerFactory()
                .CreateLogger(typeof(T));

            if (producerConfiguration.RetryProducing == true)
                unsentMessagesBuffer = new ConcurrentQueue<RetryTransmitMessage>();
        }

        /// <summary>
        /// Component Token, is needed only if the node asks for it
        /// </summary>
        /// <param name="componentToken">Component token</param>
        /// <returns>ProducerBase</returns>
        public ProducerBase<T> ComponentToken(string componentToken)
        {
            producerConfiguration.ComponentToken = componentToken;
            return this;
        }

        /// <summary>
        /// Component name where producer will produce.
        /// </summary>
        /// <param name="component">component name</param>
        /// <returns>ProducerBase</returns>
        public ProducerBase<T> Component(string component)
        {
            producerConfiguration.Component = component;
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
            producerConfiguration.Topic = topic;
            producerConfiguration.IsTopicPersistent = isTopicPersistent;

            return this;
        }

        /// <summary>
        /// Name is the producer name, is mandatory field.
        /// </summary>
        /// <param name="name">Producer name</param>
        /// <returns>ProducerBase</returns>
        public ProducerBase<T> Name(string name)
        {
            producerConfiguration.Name = name;
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
            producerConfiguration.RetryProducing = isRetryProducingActive;
            if (isRetryProducingActive == true)
                unsentMessagesBuffer = new ConcurrentQueue<RetryTransmitMessage>();

            return this;
        }

        /// <summary>
        /// Tells the producer how many times to try producing the message
        /// </summary>
        /// <param name="nTimesRetry"> int number of tries</param>
        /// <returns>ProducerBase</returns>
        public ProducerBase<T> RetryProducingMessageNTimes(int nTimesRetry)
        {
            producerConfiguration.RetryProducingMessageNTimes = nTimesRetry;
            return this;
        }

        /// <summary>
        /// Build Producer
        /// </summary>
        /// <param name="openConnection">default value is true</param>
        /// <returns>Task of ProducerBase</returns>
        public async Task<ProducerBase<T>> BuildAsync(bool openConnection = true)
        {
            producerNodeService = new ProducerNodeService(new ProducerNodeProvider(xClient.GetClientConfiguration(), producerConfiguration), xClient.GetClientConfiguration());
            producerNodeService.ProducerConnected += ProducerNodeService_ProducerConnected;
            producerNodeService.ProducerDisconnected += ProducerNodeService_ProducerDisconnected;
            producerNodeService.MessageStored += ProducerNodeService_MessageStored;
            if (openConnection == true)
                await producerNodeService.ConnectAsync();

            isConnected = true;

            isBuilt = true;

            return this;
        }

        /// <summary>
        /// Build Producer
        /// </summary>
        /// <param name="openConnection">default value is true</param>
        /// <returns>ProducerBase</returns>
        public ProducerBase<T> Build(bool openConnection = true)
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
            logger.LogWarning($"andyx-client  | Producer '{obj.ProducerName}|{obj.Id}' is disconnected");
        }

        private void ProducerNodeService_ProducerConnected(ProducerConnectedArgs obj)
        {
            logger.LogWarning($"andyx-client  | Producer '{obj.ProducerName}|{obj.Id}' is connected");
        }

        public Guid Produce(T tObject)
        {
            return ProduceAsync(tObject).Result;
        }

        public async Task<Guid> ProduceAsync(T tObject)
        {
            var message = new TransmitMessageArgs()
            {
                Id = Guid.NewGuid(),
                Tenant = xClient.GetClientConfiguration().Tenant,
                Product = xClient.GetClientConfiguration().Product,
                Component = producerConfiguration.Component,
                Topic = producerConfiguration.Topic,
                MessageRaw = tObject,
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

        private void EnqueueMessageToBuffer(TransmitMessageArgs message)
        {
            logger.LogWarning($"andyx-client  | Producing of message '{message.Id}' at {message.Tenant}/{message.Product}/{message.Component}/{message.Topic} failed, retrying 1 of {producerConfiguration.RetryProducingMessageNTimes} tires");
            if (producerConfiguration.RetryProducing == true)
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
                    if (retryTransmitMessage.RetryCounter < producerConfiguration.RetryProducingMessageNTimes)
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
                        logger.LogError($"andyx-client  | Producing of message '{retryTransmitMessage.TransmitMessageArgs.Id}' " +
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
