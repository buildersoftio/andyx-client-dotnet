using Andy.X.Client.Configurations;

namespace Andy.X.Client.Abstractions.Consumers
{
    public interface IConsumerSubscriptionTypeConnection<T>
    {
        IConsumerOtherConfiguration<T> AndSubscriptionType(SubscriptionType subscriptionType);
    }
}
