using Andy.X.Client.Configurations;
using Microsoft.AspNetCore.SignalR.Client;

namespace Andy.X.Client.Abstractions
{
    public abstract partial class ConsumerBase<T>
    {
        private class ConsumerNodeProvider
        {
            private readonly XClientConfiguration xClientConfig;
            private readonly ConsumerConfiguration consumerConfig;

            private readonly HubConnection _connection;

            public ConsumerNodeProvider(XClientConfiguration xClientConfig, ConsumerConfiguration consumerConfig)
            {
                this.xClientConfig = xClientConfig;
                this.consumerConfig = consumerConfig;

                _connection = new HubConnectionBuilder()
                     .WithUrl($"{xClientConfig.XNodeUrl}/realtime/v2/consumer", option =>
                     {
                         option.Headers["Authorization"] = $"Bearer {xClientConfig.Token}";
                         option.Headers["x-andyx-tenant"] = xClientConfig.Tenant;
                         option.Headers["x-andyx-product"] = xClientConfig.Product;
                         option.Headers["x-andyx-component"] = consumerConfig.Component;
                         option.Headers["x-andyx-topic"] = consumerConfig.Topic;
                         option.Headers["x-andyx-consumer"] = consumerConfig.Name;
                         option.Headers["x-andyx-consumer-type"] = consumerConfig.SubscriptionType.ToString();
                     })
                     .WithAutomaticReconnect()
                     .Build();
            }
            public HubConnection GetHubConnection()
            {
                return _connection;
            }
        }
    }
}
