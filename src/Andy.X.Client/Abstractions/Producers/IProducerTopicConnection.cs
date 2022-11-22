namespace Andy.X.Client.Abstractions.Producers
{
    public interface IProducerTopicConnection<K,V>
    {
        IProducerConnection<K,V> AndTopic(string topic, string description = "");
    }
}