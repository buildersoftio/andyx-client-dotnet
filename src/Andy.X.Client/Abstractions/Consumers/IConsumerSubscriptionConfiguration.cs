﻿using Andy.X.Client.Configurations;
using System;

namespace Andy.X.Client.Abstractions.Consumers
{
    public interface IConsumerSubscriptionConfiguration<K, V>
    {
        IConsumerConfiguration<K, V> AndSubscription(Action<SubscriptionConfiguration> config);
    }
}
