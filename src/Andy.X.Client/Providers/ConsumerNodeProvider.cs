using Andy.X.Client.Configurations;
using Andy.X.Client.Utilities.Helpers;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Andy.X.Client.Providers
{
    internal class ConsumerNodeProvider
    {
        private readonly XClientConfiguration _xClientConfiguration;
        private readonly ConsumerConfiguration _consumerConfiguration;

        private readonly HubConnection _connection;
        private readonly IHubConnectionBuilder _hubConnectionBuilder;

        public ConsumerNodeProvider(XClientConfiguration xClientConfiguration, ConsumerConfiguration consumerConfiguration)
        {
            _xClientConfiguration = xClientConfiguration;
            _consumerConfiguration = consumerConfiguration;

            _hubConnectionBuilder = new HubConnectionBuilder()
                .WithUrl($"{_xClientConfiguration.NodeUrl.AbsoluteUri}realtime/v3/consumer", option =>
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

                    if (_consumerConfiguration.Component.Key != "")
                        option.Headers["x-andyx-component-authoriziation"] = AuthorizationHelpers.GenerateToken(_consumerConfiguration.Component.Key, _consumerConfiguration.Component.Secret);

                    // Headers for general location
                    option.Headers["x-andyx-tenant"] = xClientConfiguration.Tenant.Name;
                    option.Headers["x-andyx-product"] = xClientConfiguration.Product.Name;
                    option.Headers["x-andyx-component"] = _consumerConfiguration.Component.Name;
                    option.Headers["x-andyx-topic"] = _consumerConfiguration.Topic.Name;
                    option.Headers["x-andyx-topic-description"] = _consumerConfiguration.Topic.Description;


                    // Consumer related headers
                    option.Headers["x-andyx-consumer-name"] = _consumerConfiguration.Name;

                    // Subscription related configuration headers
                    option.Headers["x-andyx-subscription-name"] = _consumerConfiguration.Subscription.Name;
                    option.Headers["x-andyx-subscription-mode"] = _consumerConfiguration.Subscription.Mode.ToString();
                    option.Headers["x-andyx-subscription-type"] = _consumerConfiguration.Subscription.Type.ToString();
                    option.Headers["x-andyx-subscription-initial-position"] = _consumerConfiguration.Subscription.InitialPosition.ToString();
                })
                .AddMessagePackProtocol();

            if (_xClientConfiguration.Settings.EnableAutoReconnect)
                _hubConnectionBuilder.WithAutomaticReconnect();

            // adding logging only for testing purposes.
            _hubConnectionBuilder.ConfigureLogging(builder =>
            {
                builder.AddSystemdConsole();
                builder.AddFilter("Console", level => level >= LogLevel.Information);
            });

            _connection = _hubConnectionBuilder
                .Build();
        }

        public HubConnection GetHubConnection()
        {
            return _connection;
        }

        public ConsumerConfiguration GetConsumerConfiguration()
        {
            return _consumerConfiguration;
        }
    }
}
