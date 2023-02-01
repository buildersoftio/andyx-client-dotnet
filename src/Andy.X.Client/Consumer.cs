using Andy.X.Client.Abstractions;
using Andy.X.Client.Abstractions.Base;
using Andy.X.Client.Abstractions.Consumers;
using Andy.X.Client.Abstractions.XClients;
using Andy.X.Client.Builders;
using Andy.X.Client.Configurations;

namespace Andy.X.Client
{
    public class Consumer<K, V> : ConsumerBase<K, V>
    {
        private Consumer(IXClient xClient) : base(xClient)
        {
        }
        public static IConsumerComponentConnection<K,V> CreateNewConsumer(IXClient xClient)
        {
            return new Consumer<K, V>(xClient);
        }

        private Consumer(IXClientFactory xClientFactory) : base(xClientFactory)
        {
        }
        public static IConsumerComponentConnection<K, V> CreateNewConsumer(IXClientFactory xClientFactory)
        {
            return new Consumer<K, V>(xClientFactory);
        }

        private Consumer(IXClientFactory xClientFactory, ConsumerConfiguration consumerConfiguration) : base(xClientFactory, consumerConfiguration)
        {
        }
        public static IConsumerConfiguration<K, V> CreateNewConsumer(IXClientFactory xClientFactory, ConsumerConfiguration consumerConfiguration)
        {
            return new Consumer<K, V>(xClientFactory, consumerConfiguration);
        }

        private Consumer(IXClientFactory xClientFactory, ConsumerBuilder<K, V> consumerBuilder) : base(xClientFactory, consumerBuilder)
        {
        }
        public static IConsumerConfiguration<K, V> CreateNewConsumer(IXClientFactory xClientFactory, ConsumerBuilder<K, V> consumerBuilder)
        {
            return new Consumer<K, V>(xClientFactory, consumerBuilder);
        }

        private Consumer(XClient xClient, ConsumerConfiguration consumerConfiguration) : base(xClient, consumerConfiguration)
        {
        }
        public static IConsumerConfiguration<K, V> CreateNewConsumer(XClient xClient, ConsumerConfiguration consumerConfiguration)
        {
            return new Consumer<K, V>(xClient, consumerConfiguration);
        }
    }
}
