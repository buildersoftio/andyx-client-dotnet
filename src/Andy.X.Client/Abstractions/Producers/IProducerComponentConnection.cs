namespace Andy.X.Client.Abstractions.Producers
{
    public interface IProducerComponentConnection<K, V>
    {
        IProducerTopicConnection<K, V> ForComponent(string component);
        IProducerTopicConnection<K, V> ForComponent(string component, string key, string secret);
    }
}
