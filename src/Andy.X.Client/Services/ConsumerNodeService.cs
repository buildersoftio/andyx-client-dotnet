using Andy.X.Client.Commands;
using Andy.X.Client.Configurations;
using Andy.X.Client.Events.Consumers;
using Andy.X.Client.Providers;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Andy.X.Client.Services
{
    internal class ConsumerNodeService
    {
        private readonly ILogger<ConsumerNodeService> _logger;

        private readonly HubConnection _connection;

        private readonly ConsumerNodeProvider _consumerNodeProvider;
        private readonly XClientConfiguration _xClientConfiguration;

        public event Action<ConsumerConnectedArgs> ConsumerConnected;
        public event Action<ConsumerDisconnectedArgs> ConsumerDisconnected;
        public event Action<MessageInternalReceivedArgs> MessageInternalReceived;
        public event Action<string> AndyOrderedDisconnect;

        public ConsumerNodeService(ConsumerNodeProvider consumerNodeProvider, XClientConfiguration xClientConfiguration)
        {
            _consumerNodeProvider = consumerNodeProvider;
            _xClientConfiguration = xClientConfiguration;

            _logger = xClientConfiguration
                .Settings
                .Logging
                .GetLoggerFactory()
                .CreateLogger<ConsumerNodeService>();

            _connection = consumerNodeProvider.GetHubConnection();

            _connection.Closed += Connection_Closed;
            _connection.Reconnected += Connection_Reconnected;
            _connection.Reconnecting += Connection_Reconnecting;


            _connection.On<ConsumerConnectedArgs>("ConsumerConnected", connectedArgs => ConsumerConnected?.Invoke(connectedArgs));
            _connection.On<ConsumerDisconnectedArgs>("ConsumerDisconnected", disconnected => ConsumerDisconnected?.Invoke(disconnected));
            _connection.On<MessageInternalReceivedArgs>("MessageSent", received => MessageInternalReceived?.Invoke(received));

            _connection.On<string>("AndyOrderedDisconnect", message => AndyOrderedDisconnect?.Invoke(message));
        }

        public async Task ConnectAsync()
        {
            await _connection.StartAsync().ContinueWith(async task =>
            {
                if (task.Exception != null)
                {
                    _logger.LogError($"Consumer {_consumerNodeProvider.GetConsumerConfiguration().Name} failed to connect to Andy X, details {task.Exception.Message}");
                    if (_xClientConfiguration.Settings.EnableAutoReconnect == true)
                    {
                        // retry connection
                        Thread.Sleep(_xClientConfiguration.Settings.ReconnectionTimeoutMs);
                        _logger.LogWarning($"Consumer {_consumerNodeProvider.GetConsumerConfiguration().Name} is connectiong to Andy X");
                        await ConnectAsync();
                    }
                }
            });
        }

        public async Task DisconnectAsync()
        {
            await _connection.StopAsync().ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    _logger.LogError($"Consumer {_consumerNodeProvider.GetConsumerConfiguration().Name} failed to disconnect from Andy X, details {task.Exception.Message}");
                }
            });
        }


        public async Task AcknowledgeMessage(AcknowledgeMessageArgs acknowledgeMessageArgs)
        {
            await _connection.SendAsync("AcknowledgeMessage", acknowledgeMessageArgs);
        }


        private Task Connection_Reconnecting(Exception arg)
        {
            _logger.LogWarning($"Consumer {_consumerNodeProvider.GetConsumerConfiguration().Name} connection is lost, agent is reconnecting to node, details {arg.Message}");
            return Task.CompletedTask;
        }

        private Task Connection_Reconnected(string arg)
        {
            _logger.LogInformation($"Consumer {_consumerNodeProvider.GetConsumerConfiguration().Name} is reconnected");
            return Task.CompletedTask;
        }

        private Task Connection_Closed(Exception arg)
        {
            _logger.LogError($"Consumer {_consumerNodeProvider.GetConsumerConfiguration().Name} connection is closed, details {arg.Message}");
            return Task.CompletedTask;
        }
    }
}
