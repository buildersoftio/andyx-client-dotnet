using Andy.X.Client.Configurations;
using System;

namespace Andy.X.Client.Builders
{
    public class ProducerBuilder<K, V>
    {
        public ProducerConfiguration ProducerConfiguration { get; private set; }

        public ProducerBuilder(ProducerConfiguration producerConfiguration)
        {
            ProducerConfiguration = producerConfiguration;
        }

        public ProducerBuilder(Action<ProducerConfiguration> config)
        {
            ProducerConfiguration = new ProducerConfiguration();
            config.Invoke(ProducerConfiguration);
        }
    }
}
