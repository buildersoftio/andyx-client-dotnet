using Andy.X.Client.Models;

namespace Andy.X.Client.Abstractions.Consumers
{
    public interface IConsumerMessageHandler<K,V>
    {
        Consumer<K, V> Build();
    }
}
