using Andy.X.Client.Configurations;
using Andy.X.Client.Events.Producers;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Andy.X.Client.Abstractions
{
    public abstract partial class ProducerBase<T>
    {
        private class ProducerNodeService
        {
            private readonly ILogger<ProducerNodeService> _logger;

            private readonly HubConnection _connection;
            private readonly XClientConfiguration _xClientConfiguration;

            public event Action<ProducerConnectedArgs> ProducerConnected;
            public event Action<ProducerDisconnectedArgs> ProducerDisconnected;
            public event Action<MessageStoredArgs> MessageStored;
            public event Action<string> AndyOrderedDisconnect;

            public ProducerNodeService(ProducerNodeProvider producerNodeProvider,
                XClientConfiguration xClientConfiguration)
            {
                _logger = xClientConfiguration
                    .Logging
                    .GetLoggerFactory()
                    .CreateLogger<ProducerNodeService>();

                _connection = producerNodeProvider.GetHubConnection();

                // events of producer connection
                _connection.Closed += Connection_Closed;
                _connection.Reconnected += Connection_Reconnected;
                _connection.Reconnecting += Connection_Reconnecting;

                _xClientConfiguration = xClientConfiguration;

                _connection.On<ProducerConnectedArgs>("ProducerConnected", connectedArgs => ProducerConnected?.Invoke(connectedArgs));
                _connection.On<ProducerDisconnectedArgs>("ProducerDisconnected", disconnected => ProducerDisconnected?.Invoke(disconnected));
                _connection.On<MessageStoredArgs>("MessageStored", received => MessageStored?.Invoke(received));
                _connection.On<string>("AndyOrderedDisconnect", message => AndyOrderedDisconnect?.Invoke(message));
            }

            private Task Connection_Reconnecting(Exception arg)
            {
                _logger.LogWarning($"Producer connection is lost, agent is reconnecting to node, details {arg.Message}");
                return Task.CompletedTask;
            }

            private Task Connection_Reconnected(string arg)
            {
                _logger.LogInformation($"Producer is reconnected");
                return Task.CompletedTask;
            }

            private Task Connection_Closed(Exception arg)
            {
                _logger.LogError($"Producer connection is closed, details {arg.Message}");
                return Task.CompletedTask;
            }

            public async Task ConnectAsync()
            {
                await _connection.StartAsync().ContinueWith(async task =>
                {
                    if (task.Exception != null)
                    {
                        _logger.LogError($"Producer failed to connect to Andy X Node, details {task.Exception.Message}");
                        if (_xClientConfiguration.AutoConnect == true)
                        {
                            // retry connection
                            Thread.Sleep(3000);
                            _logger.LogWarning($"Producer is connecting to Andy X Node");
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
                        _logger.LogError($"Producer failed to disconnect from Andy X Node, details {task.Exception.Message}");
                    }
                });
            }

            public async Task TransmitMessage(TransmitMessageArgs transmitMessageArgs)
            {
                await _connection.SendAsync("TransmitMessage", transmitMessageArgs);
            }

            public async Task TransmitMessages(List<TransmitMessageArgs> transmitMessageArgs)
            {
                await _connection.SendAsync("TransmitMessages", transmitMessageArgs);
            }

            public HubConnectionState GetConnectionState()
            {
                return _connection.State;
            }
        }
    }
}
