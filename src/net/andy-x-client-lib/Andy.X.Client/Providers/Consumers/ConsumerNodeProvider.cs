using Andy.X.Client.Configurations;
using MessagePack;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
                     .WithUrl($"{xClientConfig.ServiceUrl}/realtime/v3/consumer", option =>
                     {
                         option.HttpMessageHandlerFactory = (message) =>
                         {
                             return xClientConfig.HttpClientHandler;
                         };


                         option.Headers["x-andyx-tenant-authoriziation"] = xClientConfig.TenantToken;
                         option.Headers["x-andyx-component-authoriziation"] = consumerConfig.ComponentToken;

                         option.Headers["x-andyx-tenant"] = xClientConfig.Tenant;
                         option.Headers["x-andyx-product"] = xClientConfig.Product;
                         option.Headers["x-andyx-component"] = consumerConfig.Component;
                         option.Headers["x-andyx-topic"] = consumerConfig.Topic;
                         option.Headers["x-andyx-consumer-name"] = consumerConfig.Name;
                         option.Headers["x-andyx-topic-is-persistent"] = consumerConfig.IsTopicPersistent.ToString();

                         option.Headers["x-andyx-subscription-name"] = consumerConfig.SubscriptionSettings.SubscriptionName;
                         option.Headers["x-andyx-subscription-type"] = consumerConfig.SubscriptionSettings.SubscriptionType.ToString();
                         option.Headers["x-andyx-subscription-mode"] = consumerConfig.SubscriptionSettings.SubscriptionMode.ToString();
                         option.Headers["x-andyx-subscription-initial-position"] = consumerConfig.SubscriptionSettings.InitialPosition.ToString();
                     })
                     .WithAutomaticReconnect()
                     .AddMessagePackProtocol()
                     .ConfigureLogging(factory =>
                     {
                         factory.AddSystemdConsole();
                         factory.AddFilter("Console", level => level >= LogLevel.Information);
                     })
                     .Build();
            }
            public HubConnection GetHubConnection()
            {
                return _connection;
            }
        }
    }
}
