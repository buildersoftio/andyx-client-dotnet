namespace Andy.X.Client.Abstractions.Consumers
{
    public interface IConsumerComponentConnection<T>
    {
        IConsumerTopicConnection<T> ForComponent(string component);
        IConsumerTopicConnection<T> ForComponent(string component, string token);
    }
}
