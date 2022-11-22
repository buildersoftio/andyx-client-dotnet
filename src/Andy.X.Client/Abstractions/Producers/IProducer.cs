using Andy.X.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Andy.X.Client.Abstractions.Producers
{
    public interface IProducer<K, V> : IProducerConfiguration<K, V>, IProducerComponentConnection<K, V>, IProducerTopicConnection<K, V>, IProducerConnection<K, V>
    {
        Task<MessageId> SendAsync(K id, V message, IDictionary<string, string> headers = null, string sendNodeId = "");
        Task<MessageId> SendAsync(IDictionary<K, V> messages, IDictionary<string, string> headers = null, string sendNodeId = "");

        Task OpenAsync();
        Task CloseAsync();
    }
}
