using Andy.X.Client.Commands;
using Andy.X.Client.Configurations;
using Andy.X.Client.Events.Producers;
using Andy.X.Client.Providers;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Andy.X.Client.Services
{
    internal class ProducerNodeService
    {
        private readonly ILogger<ProducerNodeService> _logger;

        private readonly HubConnection _connection;

        private readonly ProducerNodeProvider _producerNodeProvider;
        private readonly XClientConfiguration _xClientConfiguration;

        public event Action<ProducerConnectedArgs> ProducerConnected;
        public event Action<ProducerDisconnectedArgs> ProducerDisconnected;
        public event Action<MessageAcceptedArgs> MessageAccepted;
        public event Action<string> AndyOrderedDisconnect;

        public ProducerNodeService(ProducerNodeProvider producerNodeProvider, XClientConfiguration xClientConfiguration)
        {
            _producerNodeProvider = producerNodeProvider;
            _xClientConfiguration = xClientConfiguration;

            _logger = xClientConfiguration
                    .Settings
                    .Logging
                    .GetLoggerFactory()
                    .CreateLogger<ProducerNodeService>();

            _connection = producerNodeProvider.GetHubConnection();

            _connection.Closed += Connection_Closed;
            _connection.Reconnected += Connection_Reconnected;
            _connection.Reconnecting += Connection_Reconnecting;

            _connection.On<ProducerConnectedArgs>("ProducerConnected", connectedArgs => ProducerConnected?.Invoke(connectedArgs));
            _connection.On<ProducerDisconnectedArgs>("ProducerDisconnected", disconnected => ProducerDisconnected?.Invoke(disconnected));
            _connection.On<MessageAcceptedArgs>("MessageAccepted", msgAccepted => MessageAccepted?.Invoke(msgAccepted));

            _connection.On<string>("AndyOrderedDisconnect", message => AndyOrderedDisconnect?.Invoke(message));

        }

        public async Task ConnectAsync()
        {
            await _connection.StartAsync().ContinueWith(async task =>
            {
                if (task.Exception != null)
                {
                    _logger.LogError($"Producer {_producerNodeProvider.GetProducerConfiguration().Name} failed to connect to Andy X Node, details {task.Exception.Message}");
                    if (_xClientConfiguration.Settings.EnableAutoReconnect == true)
                    {
                        Thread.Sleep(_xClientConfiguration.Settings.ReconnectionTimeoutMs);
                        _logger.LogWarning($"Producer {_producerNodeProvider.GetProducerConfiguration().Name} is connecting to Andy X");

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
                    _logger.LogError($"Producer {_producerNodeProvider.GetProducerConfiguration().Name} failed to disconnect from Andy X, details {task.Exception.Message}");
                }
            });
        }

        public async Task TransmitMessage(TransmitMessageArgs transmitMessageArgs)
        {
            await _connection.SendAsync("TransmitMessage", transmitMessageArgs);
        }

        public async Task TransmitMessages(List<TransmitMessageArgs> transmitMessagesArgs)
        {
            await _connection.SendAsync("TransmitMessages", transmitMessagesArgs);
        }


        private Task Connection_Reconnecting(Exception arg)
        {
            _logger.LogWarning($"Producer {_producerNodeProvider.GetProducerConfiguration().Name} connection is lost, agent is reconnecting to node, details {arg.Message}");
            return Task.CompletedTask;
        }

        private Task Connection_Reconnected(string arg)
        {
            _logger.LogInformation($"Producer {_producerNodeProvider.GetProducerConfiguration().Name} is reconnected");
            return Task.CompletedTask;
        }

        private Task Connection_Closed(Exception arg)
        {
            _logger.LogError($"Producer {_producerNodeProvider.GetProducerConfiguration().Name} connection is closed, details {arg.Message}");
            return Task.CompletedTask;
        }

        public HubConnectionState GetConnectionState()
        {
            return _connection.State;
        }
    }
}
