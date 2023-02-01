using Andy.X.Client.Abstractions.Serializers;
using Andy.X.Client.Configurations;
using MessagePack;

namespace Andy.X.Client.Serializers
{
    public class DefaultContractlessMessageSerializer : IMessageSerializer
    {
        private readonly MessagePackSerializerOptions _messagePackSerializerOptions;

        public DefaultContractlessMessageSerializer(CompressionType compressionType)
        {
            switch (compressionType)
            {
                case CompressionType.None:
                    _messagePackSerializerOptions = MessagePack.Resolvers.ContractlessStandardResolver.Options.WithCompression(MessagePackCompression.None);
                    break;
                case CompressionType.Lz4Block:
                    _messagePackSerializerOptions = MessagePack.Resolvers.ContractlessStandardResolver.Options.WithCompression(MessagePackCompression.Lz4Block);
                    break;
                case CompressionType.Lz4BlockArray:
                    _messagePackSerializerOptions = MessagePack.Resolvers.ContractlessStandardResolver.Options.WithCompression(MessagePackCompression.Lz4BlockArray);
                    break;
                default:
                    _messagePackSerializerOptions = MessagePack.Resolvers.ContractlessStandardResolver.Options.WithCompression(MessagePackCompression.None);
                    break;
            }
        }

        public (K key, V value) Deserialize<K, V>(byte[] key, byte[] value)
        {
            K keyObject = MessagePackSerializer.Deserialize<K>(key, _messagePackSerializerOptions);
            V valueObject = MessagePackSerializer.Deserialize<V>(key, _messagePackSerializerOptions);

            return (keyObject, valueObject);
        }

        public (byte[] key, byte[] value) Serialize<K, V>(K key, V value)
        {
            var keySerialized = MessagePackSerializer.Serialize(key, _messagePackSerializerOptions);
            var valueSerialized = MessagePackSerializer.Serialize(value, _messagePackSerializerOptions);

            return (keySerialized, valueSerialized);
        }
    }
}
