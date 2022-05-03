namespace Andy.X.Client.Abstractions.Producers
{
    public interface IProducerNameConnection<T>
    {
        IProducerOtherConfiguration<T> WithName(string name);
    }
}
