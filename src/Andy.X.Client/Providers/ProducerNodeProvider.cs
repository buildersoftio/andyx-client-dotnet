using Andy.X.Client.Configurations;
using Andy.X.Client.Utilities.Helpers;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Andy.X.Client.Providers
{
    internal class ProducerNodeProvider
    {
        private readonly XClientConfiguration _xClientConfiguration;
        private readonly ProducerConfiguration _producerConfiguration;

        private readonly HubConnection _connection;
        private readonly IHubConnectionBuilder _hubConnectionBuilder;

        public ProducerNodeProvider(XClientConfiguration xClientConfiguration, ProducerConfiguration producerConfiguration)
        {
            _xClientConfiguration = xClientConfiguration;
            _producerConfiguration = producerConfiguration;

            _hubConnectionBuilder = new HubConnectionBuilder()
                .WithUrl($"{_xClientConfiguration.NodeUrl.AbsoluteUri}realtime/v3/producer", option =>
                {
                    // Implement HttpMessageHandler given from the client.
                    option.HttpMessageHandlerFactory = (message) =>
                    {
                        return xClientConfiguration.Settings.HttpClientHandler;
                    };

                    // Authorization tokens for tenant, product and component
                    if (_xClientConfiguration.Tenant.Key != "")
                        option.Headers["x-andyx-tenant-authoriziation"] = AuthorizationHelpers.GenerateToken(_xClientConfiguration.Tenant.Key, _xClientConfiguration.Tenant.Secret);

                    if (_xClientConfiguration.Product.Key != "")
                        option.Headers["x-andyx-product-authoriziation"] = AuthorizationHelpers.GenerateToken(_xClientConfiguration.Product.Key, _xClientConfiguration.Product.Secret);

                    if (_producerConfiguration.Component.Key != "")
                        option.Headers["x-andyx-component-authoriziation"] = AuthorizationHelpers.GenerateToken(_producerConfiguration.Component.Key, _producerConfiguration.Component.Secret);

                    // Headers for general location
                    option.Headers["x-andyx-tenant"] = xClientConfiguration.Tenant.Name;
                    option.Headers["x-andyx-product"] = xClientConfiguration.Product.Name;
                    option.Headers["x-andyx-component"] = _producerConfiguration.Component.Name;
                    option.Headers["x-andyx-topic"] = _producerConfiguration.Topic.Name;
                    option.Headers["x-andyx-topic-description"] = _producerConfiguration.Topic.Description;

                    option.Headers["x-andyx-producer-name"] = _producerConfiguration.Name;

                })
            .AddMessagePackProtocol();


            if (_xClientConfiguration.Settings.EnableAutoReconnect)
                _hubConnectionBuilder.WithAutomaticReconnect();


            _connection = _hubConnectionBuilder
                .Build();
        }

        public HubConnection GetHubConnection()
        {
            return _connection;
        }

        public ProducerConfiguration GetProducerConfiguration()
        {
            return _producerConfiguration;
        }
    }
}
