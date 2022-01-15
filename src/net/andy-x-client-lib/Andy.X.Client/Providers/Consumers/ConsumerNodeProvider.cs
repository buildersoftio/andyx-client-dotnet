using Andy.X.Client.Configurations;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;

namespace Andy.X.Client.Abstractions
{
    public abstract partial class ConsumerBase<T>
    {
        private class ConsumerNodeProvider
        {
            private readonly XClientConfiguration xClientConfig;
            private readonly ConsumerConfiguration<T> consumerConfig;

            private readonly HubConnection _connection;

            public ConsumerNodeProvider(XClientConfiguration xClientConfig, ConsumerConfiguration<T> consumerConfig)
            {
                this.xClientConfig = xClientConfig;
                this.consumerConfig = consumerConfig;

                _connection = new HubConnectionBuilder()
                     .WithUrl($"{xClientConfig.ServiceUrl}/realtime/v2/consumer", option =>
                     {
                         option.HttpMessageHandlerFactory = (message) =>
                         {
                             return xClientConfig.HttpClientHandler;
                         };

                         option.Headers["Authorization"] = $"Bearer {xClientConfig.Token}";
                         option.Headers["x-andyx-tenant"] = xClientConfig.Tenant;
                         option.Headers["x-andyx-product"] = xClientConfig.Product;
                         option.Headers["x-andyx-component"] = consumerConfig.Component;
                         option.Headers["x-andyx-topic"] = consumerConfig.Topic;
                         option.Headers["x-andyx-consumer"] = consumerConfig.Name;
                         option.Headers["x-andyx-consumer-type"] = consumerConfig.SubscriptionType.ToString();

                         option.Headers["x-andyx-topic-is-persistent"] = consumerConfig.IsTopicPersistent.ToString();
                         option.Headers["x-andyx-consumer-initial-position"] = consumerConfig.InitialPosition.ToString();
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
