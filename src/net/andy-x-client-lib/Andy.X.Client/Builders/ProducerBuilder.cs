using Andy.X.Client.Configurations;
using System;

namespace Andy.X.Client.Builders
{
    public class ProducerBuilder<T>
    {
        public ProducerConfiguration<T> ProducerConfiguration { get; private set; }

        public ProducerBuilder(ProducerConfiguration<T> producerConfiguration)
        {
            ProducerConfiguration = producerConfiguration;
        }

        public ProducerBuilder(Action<ProducerConfiguration<T>> config)
        {
            ProducerConfiguration = new ProducerConfiguration<T>();
            config.Invoke(ProducerConfiguration);
        }
    }
}
