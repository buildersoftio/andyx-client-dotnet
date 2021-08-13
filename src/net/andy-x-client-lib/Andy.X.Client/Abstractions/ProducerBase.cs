using Andy.X.Client.Configurations;
using Andy.X.Client.Events.Producers;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace Andy.X.Client.Abstractions
{
    public abstract partial class ProducerBase<T>
    {
        private readonly XClient xClient;
        private readonly ProducerConfiguration producerConfiguration;

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
            producerConfiguration = new ProducerConfiguration();
        }
        public ProducerBase(XClient xClient, ProducerConfiguration producerConfiguration)
        {
            this.xClient = xClient;
            this.producerConfiguration = producerConfiguration;
            if (producerConfiguration.RetryProducing == true)
                unsentMessagesBuffer = new ConcurrentQueue<RetryTransmitMessage>();
        }
        public ProducerBase(IXClientFactory xClient)
        {
            this.xClient = xClient.CreateClient();
            producerConfiguration = new ProducerConfiguration();
        }
        public ProducerBase(IXClientFactory xClient, ProducerConfiguration producerConfiguration)
        {
            this.xClient = xClient.CreateClient();
            this.producerConfiguration = producerConfiguration;

            if (producerConfiguration.RetryProducing == true)
                unsentMessagesBuffer = new ConcurrentQueue<RetryTransmitMessage>();
        }


        public ProducerBase<T> Component(string component)
        {
            producerConfiguration.Component = component;
            return this;
        }

        public ProducerBase<T> Topic(string topic)
        {
            producerConfiguration.Topic = topic;
            return this;
        }

        public ProducerBase<T> Name(string name)
        {
            producerConfiguration.Name = name;
            return this;
        }

        public ProducerBase<T> RetryProducing(bool isRetryProducingActive)
        {
            producerConfiguration.RetryProducing = isRetryProducingActive;
            if (isRetryProducingActive == true)
                unsentMessagesBuffer = new ConcurrentQueue<RetryTransmitMessage>();

            return this;
        }

        public ProducerBase<T> RetryProducingMessageNTimes(int nTimesRetry)
        {
            producerConfiguration.RetryProducingMessageNTimes = nTimesRetry;
            return this;
        }

        public async Task<ProducerBase<T>> BuildAsync()
        {
            producerNodeService = new ProducerNodeService(new ProducerNodeProvider(xClient.GetClientConfiguration(), producerConfiguration));
            producerNodeService.ProducerConnected += ProducerNodeService_ProducerConnected;
            producerNodeService.ProducerDisconnected += ProducerNodeService_ProducerDisconnected;
            producerNodeService.MessageStored += ProducerNodeService_MessageStored;
            await producerNodeService.ConnectAsync();
            isConnected = true;

            isBuilt = true;

            return this;
        }

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
            this.MessageStored?.Invoke(this, obj);
        }

        private void ProducerNodeService_ProducerDisconnected(ProducerDisconnectedArgs obj)
        {
            Console.WriteLine($"andyx|{obj.Tenant}|{obj.Product}|{obj.Component}|{obj.Topic}|producers#{obj.ProducerName}|{obj.Id}|disconnected");
        }

        private void ProducerNodeService_ProducerConnected(ProducerConnectedArgs obj)
        {
            Console.WriteLine($"andyx|{obj.Tenant}|{obj.Product}|{obj.Component}|{obj.Topic}|producers#{obj.ProducerName}|{obj.Id}|connected");
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
                MessageRaw = tObject
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

                    // If RetryCounter is bigger than RetryProducerMessageNTimes ignore that message.

                }
            }

            isUnsentMessagesProcessorWorking = false;
        }
        #endregion
    }
}
