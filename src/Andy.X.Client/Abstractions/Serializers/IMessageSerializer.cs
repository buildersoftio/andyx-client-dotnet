namespace Andy.X.Client.Abstractions.Serializers
{
    public interface IMessageSerializer
    {
        (byte[] key, byte[] value) Serialize<K, V>(K key, V value);
        (K key, V value) Deserialize<K, V>(byte[] key, byte[] value);
    }
}
