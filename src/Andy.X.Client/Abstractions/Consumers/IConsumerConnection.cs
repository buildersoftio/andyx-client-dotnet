namespace Andy.X.Client.Abstractions.Consumers
{
    public interface IConsumerConnection<K,V>
    {
        IConsumerSubscriptionConfiguration<K,V> WithName(string name);
    }
}