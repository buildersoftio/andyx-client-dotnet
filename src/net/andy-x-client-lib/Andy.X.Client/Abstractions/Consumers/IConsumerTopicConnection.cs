namespace Andy.X.Client.Abstractions.Consumers
{
    public interface IConsumerTopicConnection<T>
    {
        IConsumerNameConnection<T> AndTopic(string topic);
        IConsumerNameConnection<T> AndTopic(string topic, bool isPersistent);
    }
}
