using Andy.X.Client.Abstractions;
using Andy.X.Client.Abstractions.Producers;
using Andy.X.Client.Builders;
using Andy.X.Client.Configurations;

namespace Andy.X.Client
{
    public class Producer<T> : ProducerBase<T>, IProducer<T>
    {
        private Producer(XClient xClient) : base(xClient) { }
        public static IProducerComponentConnection<T> CreateNewProducer(XClient xClient)
        {
            return new Producer<T>(xClient);
        }

        private Producer(IXClientFactory xClient) : base(xClient) { }
        public static IProducerComponentConnection<T> CreateNewProducer(IXClientFactory xClient)
        {
            return new Producer<T>(xClient);
        }

        private Producer(XClient xClient, ProducerConfiguration<T> producerConfiguration) : base(xClient, producerConfiguration) { }
        public static IProducerComponentConnection<T> CreateNewProducer(XClient xClient, ProducerConfiguration<T> producerConfiguration)
        {
            return new Producer<T>(xClient, producerConfiguration);
        }

        private Producer(IXClientFactory xClient, ProducerConfiguration<T> producerConfiguration) : base(xClient, producerConfiguration) { }
        public static IProducerComponentConnection<T> CreateNewProducer(IXClientFactory xClient, ProducerConfiguration<T> producerConfiguration)
        {
            return new Producer<T>(xClient, producerConfiguration);
        }

        private Producer(IXClientFactory xClient, ProducerBuilder<T> producerBuilder) : base(xClient, producerBuilder) { }
        public static IProducerComponentConnection<T> CreateNewProducer(IXClientFactory xClient, ProducerBuilder<T> producerBuilder)
        {
            return new Producer<T>(xClient, producerBuilder);
        }
    }
}
