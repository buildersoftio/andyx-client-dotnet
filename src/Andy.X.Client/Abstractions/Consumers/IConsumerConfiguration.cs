using Andy.X.Client.Configurations;
using System;

namespace Andy.X.Client.Abstractions.Consumers
{
    public interface IConsumerConfiguration<K,V>
    {
        // we do not have settings for consumers for now.
        IConsumerConfiguration<K, V> WithSettings(Action<ConsumerSettings> settings);
        Consumer<K, V> Build();
    }
}