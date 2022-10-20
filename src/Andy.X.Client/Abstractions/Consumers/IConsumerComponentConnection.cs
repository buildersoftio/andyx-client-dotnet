namespace Andy.X.Client.Abstractions.Consumers
{
    public interface IConsumerComponentConnection<K, V>
    {
        IConsumerTopicConnection<K, V> ForComponent(string component);
        IConsumerTopicConnection<K, V> ForComponent(string component, string key, string secret);
    }
}
