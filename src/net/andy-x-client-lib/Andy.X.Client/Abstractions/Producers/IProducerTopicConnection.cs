namespace Andy.X.Client.Abstractions.Producers
{
    public interface IProducerTopicConnection<T>
    {
        IProducerNameConnection<T> AndTopic(string topic);
        IProducerNameConnection<T> AndTopic(string topic, bool isPersistent);
    }
}
