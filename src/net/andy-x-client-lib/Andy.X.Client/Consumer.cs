using Andy.X.Client.Abstractions;
using Andy.X.Client.Builders;
using Andy.X.Client.Configurations;

namespace Andy.X.Client
{
    public class Consumer<T> : ConsumerBase<T>, IConsumer<T>
    {
        public Consumer(XClient xClient) : base(xClient)
        {
        }

        public Consumer(IXClientFactory xClient) : base(xClient)
        {
        }

        public Consumer(XClient xClient, ConsumerConfiguration<T> consumerConfiguration) : base(xClient, consumerConfiguration)
        {
        }

        public Consumer(IXClientFactory xClient, ConsumerConfiguration<T> consumerConfiguration) : base(xClient, consumerConfiguration)
        {
        }

        public Consumer(IXClientFactory xClient, ConsumerBuilder<T> consumerBuilder) : base(xClient, consumerBuilder)
        {
        }
    }
}
