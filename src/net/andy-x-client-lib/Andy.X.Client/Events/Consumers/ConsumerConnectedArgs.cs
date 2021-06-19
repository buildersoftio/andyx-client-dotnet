using Andy.X.Client.Configurations;
using System;


namespace Andy.X.Client.Abstractions
{
    public abstract partial class ConsumerBase<T>
    {
        private class ConsumerConnectedArgs
        {
            public string Tenant { get; set; }
            public string Product { get; set; }
            public string Component { get; set; }
            public string Topic { get; set; }

            public Guid Id { get; set; }
            public string ConsumerName { get; set; }
            public SubscriptionType SubscriptionType { get; set; }
        }
    }
}
