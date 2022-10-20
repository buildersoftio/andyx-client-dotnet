namespace Andy.X.Client.Abstractions.Consumers
{
    public interface IConsumerTopicConnection<K,V>
    {
        IConsumerConnection<K,V> AndTopic(string topic, string description = "");
    }
}