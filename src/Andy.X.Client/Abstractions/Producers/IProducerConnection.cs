namespace Andy.X.Client.Abstractions.Producers
{
    public interface IProducerConnection<K,V>
    {
        IProducerConfiguration<K,V> WithName(string name);
    }
}