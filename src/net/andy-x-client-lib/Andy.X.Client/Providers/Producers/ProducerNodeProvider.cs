using Andy.X.Client.Configurations;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;

namespace Andy.X.Client.Abstractions
{
    public abstract partial class ProducerBase<T>
    {
        private class ProducerNodeProvider
        {
            private readonly XClientConfiguration xClientConfig;
            private readonly ProducerConfiguration producerConfig;

            private readonly HubConnection _connection;

            public ProducerNodeProvider(XClientConfiguration xClientConfig, ProducerConfiguration producerConfig)
            {
                this.xClientConfig = xClientConfig;
                this.producerConfig = producerConfig;

                _connection = new HubConnectionBuilder()
                    .WithUrl($"{xClientConfig.ServiceUrl}/realtime/v2/producer", option =>
                    {
                        option.HttpMessageHandlerFactory = (message) =>
                        {
                            if (message is HttpClientHandler httpClientHandler)
                                httpClientHandler.ServerCertificateCustomValidationCallback +=
                                    (sender, certificate, chain, sslPolicyErrors) => { return true; };
                            return message;
                        };

                        option.Headers["Authorization"] = $"Bearer {xClientConfig.Token}";
                        option.Headers["x-andyx-tenant"] = xClientConfig.Tenant;
                        option.Headers["x-andyx-product"] = xClientConfig.Product;
                        option.Headers["x-andyx-component"] = producerConfig.Component;
                        option.Headers["x-andyx-topic"] = producerConfig.Topic;
                        option.Headers["x-andyx-producer"] = producerConfig.Name;

                        option.Headers["x-andyx-topic-is-persistent"] = producerConfig.IsTopicPersistent.ToString();
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
