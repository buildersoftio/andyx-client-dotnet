using Andy.X.Client.Abstractions;
using Andy.X.Client.Configurations;

namespace Andy.X.Client
{
    public class Consumer<T> : ConsumerBase<T>
    {
        public Consumer(XClient xClient) : base(xClient)
        {
        }

        public Consumer(IXClientFactory xClient) : base(xClient)
        {
        }

        public Consumer(XClient xClient, ProducerConfiguration producerConfiguration) : base(xClient, producerConfiguration)
        {
        }

        public Consumer(IXClientFactory xClient, ProducerConfiguration producerConfiguration) : base(xClient, producerConfiguration)
        {
        }
    }
}
