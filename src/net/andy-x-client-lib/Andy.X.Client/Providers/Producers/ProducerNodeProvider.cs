using Andy.X.Client.Configurations;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Andy.X.Client.Abstractions
{
    public abstract partial class ProducerBase<T>
    {
        private class ProducerNodeProvider
        {
            private readonly XClientConfiguration xClientConfig;
            private readonly ProducerConfiguration<T> producerConfig;

            private readonly HubConnection _connection;

            public ProducerNodeProvider(XClientConfiguration xClientConfig, ProducerConfiguration<T> producerConfig)
            {
                this.xClientConfig = xClientConfig;
                this.producerConfig = producerConfig;

                _connection = new HubConnectionBuilder()
                    .WithUrl($"{xClientConfig.ServiceUrl}/realtime/v3/producer", option =>
                    {
                        //option.HttpMessageHandlerFactory = (message) =>
                        //{
                        //    return xClientConfig.HttpClientHandler;
                        //};


                        option.HttpMessageHandlerFactory = (message) =>
                        {
                            if (message is HttpClientHandler clientHandler)
                                // always verify the SSL certificate
                                clientHandler.ServerCertificateCustomValidationCallback +=
                                    (sender, certificate, chain, sslPolicyErrors) => { return true; };
                            return message;
                        };

                        option.Headers["x-andyx-tenant-authoriziation"] = xClientConfig.TenantToken;
                        option.Headers["x-andyx-component-authoriziation"] = producerConfig.ComponentToken;

                        option.Headers["x-andyx-tenant"] = xClientConfig.Tenant;
                        option.Headers["x-andyx-product"] = xClientConfig.Product;
                        option.Headers["x-andyx-component"] = producerConfig.Component;
                        option.Headers["x-andyx-topic"] = producerConfig.Topic;
                        option.Headers["x-andyx-producer-name"] = producerConfig.Name;

                        option.Headers["x-andyx-topic-is-persistent"] = producerConfig.IsTopicPersistent.ToString();
                    })
                    .WithAutomaticReconnect()
                    .AddMessagePackProtocol()
                    .ConfigureLogging(factory =>
                    {
                        factory.AddConsole();
                        factory.AddFilter("Console", level => level >= LogLevel.Trace);
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
