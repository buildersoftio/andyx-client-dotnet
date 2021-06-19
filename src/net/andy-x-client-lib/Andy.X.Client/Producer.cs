using Andy.X.Client.Abstractions;
using Andy.X.Client.Configurations;

namespace Andy.X.Client
{
    public class Producer<T> : ProducerBase<T>
    {
        public Producer(XClient xClient) : base(xClient)
        {
        }

        public Producer(IXClientFactory xClient) : base(xClient)
        {
        }

        public Producer(XClient xClient, ProducerConfiguration producerConfiguration) : base(xClient, producerConfiguration)
        {
        }

        public Producer(IXClientFactory xClient, ProducerConfiguration producerConfiguration) : base(xClient, producerConfiguration)
        {
        }
    }
}
