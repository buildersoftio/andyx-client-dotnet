﻿using Andy.X.Client.Configurations;

namespace Andy.X.Client.Abstractions.Consumers
{
    public interface IConsumerInitialPositionConnection<T>
    {
        IConsumerSubscriptionTypeConnection<T> WithInitialPosition(InitialPosition initialPosition);
    }
}