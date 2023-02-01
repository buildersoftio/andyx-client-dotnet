using Andy.X.Client.Abstractions;
using Andy.X.Client.Abstractions.Base;
using Andy.X.Client.Abstractions.Producers;
using Andy.X.Client.Abstractions.XClients;
using Andy.X.Client.Builders;
using Andy.X.Client.Configurations;

namespace Andy.X.Client
{
    public class Producer<K, V> : ProducerBase<K, V>
    {
        private Producer(IXClient xClient) : base(xClient)
        {
        }
        public static IProducerComponentConnection<K, V> CreateNewProducer(IXClient xClient)
        {
            return new Producer<K, V>(xClient);
        }

        private Producer(IXClientFactory xClientFactory) : base(xClientFactory)
        {
        }
        public static IProducerComponentConnection<K, V> CreateNewProducer(IXClientFactory xClientFactory)
        {
            return new Producer<K, V>(xClientFactory);
        }

        private Producer(IXClientFactory xClientFactory, ProducerConfiguration producerConfiguration) : base(xClientFactory, producerConfiguration)
        {
        }

        public static IProducerComponentConnection<K, V> CreateNewProducer(IXClientFactory xClientFactory, ProducerConfiguration producerConfiguration)
        {
            return new Producer<K, V>(xClientFactory, producerConfiguration);
        }

        private Producer(IXClientFactory xClientFactory, ProducerBuilder<K, V> producerBuilder) : base(xClientFactory, producerBuilder)
        {
        }
        public static IProducerConfiguration<K, V> CreateNewProducer(IXClientFactory xClientFactory, ProducerBuilder<K, V> producerBuilder)
        {
            return new Producer<K, V>(xClientFactory, producerBuilder);
        }

        private Producer(XClient xClient, ProducerConfiguration producerConfiguration) : base(xClient, producerConfiguration)
        {
        }
        public static IProducerConfiguration<K, V> CreateNewProducer(XClient xClient, ProducerConfiguration producerConfiguration)
        {
            return new Producer<K, V>(xClient, producerConfiguration);
        }
    }
}
