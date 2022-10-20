using Andy.X.Client.Configurations;
using System;

namespace Andy.X.Client.Builders
{
    public class ConsumerBuilder<K, V>
    {
        public ConsumerConfiguration ConsumerConfiguration { get; private set; }

        public ConsumerBuilder(ConsumerConfiguration consumerConfiguration)
        {
            ConsumerConfiguration = consumerConfiguration;
        }

        public ConsumerBuilder(Action<ConsumerConfiguration> config)
        {
            ConsumerConfiguration = new ConsumerConfiguration();
            config.Invoke(ConsumerConfiguration);
        }
    }
}
