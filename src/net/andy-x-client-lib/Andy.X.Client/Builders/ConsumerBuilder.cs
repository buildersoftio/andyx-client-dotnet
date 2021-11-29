using Andy.X.Client.Configurations;
using System;

namespace Andy.X.Client.Builders
{
    public class ConsumerBuilder<T>
    {
        public ConsumerConfiguration<T> ConsumerConfiguration { get; private set; }

        public ConsumerBuilder(ConsumerConfiguration<T> consumerConfiguration)
        {
            ConsumerConfiguration = consumerConfiguration;
        }

        public ConsumerBuilder(Action<ConsumerConfiguration<T>> config)
        {
            ConsumerConfiguration = new ConsumerConfiguration<T>();
            config.Invoke(ConsumerConfiguration);
        }
    }
}
