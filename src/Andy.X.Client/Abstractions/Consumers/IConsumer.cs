using Andy.X.Client.Commands;
using Andy.X.Client.Models;
using System;
using System.Threading.Tasks;

namespace Andy.X.Client.Abstractions.Consumers
{
    public interface IConsumer<K, V> : IConsumerComponentConnection<K, V>, IConsumerConfiguration<K, V>, IConsumerConnection<K, V>, IConsumerSubscriptionConfiguration<K, V>, IConsumerTopicConnection<K, V>, IConsumerMessageHandler<K, V>
    {
        IConsumerMessageHandler<K, V> MessageReceivedHandler(Action<K, Message<V>> messageReceived);

        Task AcknowledgeMessage(Message<V> message);
        Task UnacknowledgeMessage(Message<V> message);
        Task SkipMessage(Message<V> message);

        Task MessageAcknowledgement(MessageAcknowledgement messageAcknowledgement, Message<V> message);

        Task SubscribeAsync();
        Task CloseAsync();
    }
}
