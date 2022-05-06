namespace Andy.X.Client.Abstractions.Consumers
{
    public interface IConsumerNameConnection<T>
    {
        IConsumerInitialPositionConnection<T> WithName(string name);
    }
}
