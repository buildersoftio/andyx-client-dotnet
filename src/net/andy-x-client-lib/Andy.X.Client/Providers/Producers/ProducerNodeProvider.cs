using Andy.X.Client.Configurations;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    .WithUrl($"{xClientConfig.XNodeUrl}/realtime/v2/producer", option =>
                    {
                        option.Headers["Authorization"] = $"Bearer {xClientConfig.Token}";
                        option.Headers["x-andyx-tenant"] = xClientConfig.Tenant;
                        option.Headers["x-andyx-product"] = xClientConfig.Product;
                        option.Headers["x-andyx-component"] = producerConfig.Component;
                        option.Headers["x-andyx-topic"] = producerConfig.Topic;
                        option.Headers["x-andyx-producer"] = producerConfig.Name;
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
