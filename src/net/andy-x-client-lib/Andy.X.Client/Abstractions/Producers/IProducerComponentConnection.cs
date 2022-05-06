namespace Andy.X.Client.Abstractions.Producers
{
    public interface IProducerComponentConnection<T>
    {
        IProducerTopicConnection<T> ForComponent(string component);
        IProducerTopicConnection<T> ForComponent(string component, string token);
    }
}
