using Andy.X.Client.Abstractions;
using Andy.X.Client.Abstractions.Consumers;
using Andy.X.Client.Builders;
using Andy.X.Client.Configurations;

namespace Andy.X.Client
{
    public class Consumer<T> : ConsumerBase<T>, IConsumer<T>
    {
        private Consumer(XClient xClient) : base(xClient) { }

        public static IConsumerComponentConnection<T> CreateNewConsumer(XClient xClient)
        {
            return new Consumer<T>(xClient);
        }

        private Consumer(IXClientFactory xClient) : base(xClient) { }
        public static IConsumerComponentConnection<T> CreateNewConsumer(IXClientFactory xClient)
        {
            return new Consumer<T>(xClient);
        }


        private Consumer(XClient xClient, ConsumerConfiguration<T> consumerConfiguration) : base(xClient, consumerConfiguration) { }
        public static IConsumerComponentConnection<T> CreateNewConsumer(XClient xClient, ConsumerConfiguration<T> consumerConfiguration)
        {
            return new Consumer<T>(xClient, consumerConfiguration);
        }

        private Consumer(IXClientFactory xClient, ConsumerConfiguration<T> consumerConfiguration) : base(xClient, consumerConfiguration) { }
        public static IConsumerComponentConnection<T> CreateNewConsumer(IXClientFactory xClient, ConsumerConfiguration<T> consumerConfiguration)
        {
            return new Consumer<T>(xClient, consumerConfiguration);
        }

        private Consumer(IXClientFactory xClient, ConsumerBuilder<T> consumerBuilder) : base(xClient, consumerBuilder) { }
        public static IConsumerComponentConnection<T> CreateNewConsumer(IXClientFactory xClient, ConsumerBuilder<T> consumerBuilder)
        {
            return new Consumer<T>(xClient, consumerBuilder);
        }
    }
}
