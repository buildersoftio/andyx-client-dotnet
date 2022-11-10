using Andy.X.Client.Configurations;
using System;

namespace Andy.X.Client.Abstractions.Consumers
{
    public interface IConsumerSettings<K,V>
    {
        IConsumerConfiguration<K, V> WithSettings(Action<ConsumerSettings> settings);
    }
}
