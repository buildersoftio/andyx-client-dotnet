using Andy.X.Client.Configurations;
using Andy.X.Client.Events.Producers;
using System;
using System.Threading.Tasks;

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

        public ProducerBase(XClient xClient)
        {
            this.xClient = xClient;
            producerConfiguration = new ProducerConfiguration();
        }
        public ProducerBase(XClient xClient, ProducerConfiguration producerConfiguration)
        {
            this.xClient = xClient;
            this.producerConfiguration = producerConfiguration;
        }
        public ProducerBase(IXClientFactory xClient)
        {
            this.xClient = xClient.CreateClient();
            producerConfiguration = new ProducerConfiguration();
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
                await producerNodeService.CloseConnectionAsync();
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

            await producerNodeService.TransmitMessage(message);
            return message.Id;
        }
    }
}
