using Andy.X.Client.Configurations;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Andy.X.Client.Abstractions
{
    public abstract partial class ConsumerBase<T>
    {
        private class ConsumerNodeService
        {
            private readonly ILogger<ConsumerNodeService> _logger;

            private readonly HubConnection _connection;
            private readonly XClientConfiguration _xClientConfiguration;

            public event Action<ConsumerConnectedArgs> ConsumerConnected;
            public event Action<ConsumerDisconnectedArgs> ConsumerDisconnected;
            public event Action<MessageInternalReceivedArgs> MessageInternalReceived;
            public event Action<string> AndyOrderedDisconnect;

            public ConsumerNodeService(ConsumerNodeProvider consumerNodeProvider,
                XClientConfiguration xClientConfiguration)
            {

                _logger = xClientConfiguration
                    .Logging
                    .GetLoggerFactory()
                    .CreateLogger<ConsumerNodeService>();

                _connection = consumerNodeProvider.GetHubConnection();

                // events of consumer connection
                _connection.Closed += Connection_Closed;
                _connection.Reconnected += Connection_Reconnected;
                _connection.Reconnecting += Connection_Reconnecting;

                _xClientConfiguration = xClientConfiguration;

                _connection.On<ConsumerConnectedArgs>("ConsumerConnected", connectedArgs => ConsumerConnected?.Invoke(connectedArgs));
                _connection.On<ConsumerDisconnectedArgs>("ConsumerDisconnected", disconnected => ConsumerDisconnected?.Invoke(disconnected));
                _connection.On<MessageInternalReceivedArgs>("MessageSent", received => MessageInternalReceived?.Invoke(received));

                _connection.On<string>("AndyOrderedDisconnect", message => AndyOrderedDisconnect?.Invoke(message));
            }

            private Task Connection_Reconnecting(Exception arg)
            {
                _logger.LogWarning($"Consumer connection is lost, agent is reconnecting to node, details {arg.Message}");
                return Task.CompletedTask;
            }

            private Task Connection_Reconnected(string arg)
            {
                _logger.LogInformation($"Consumer is reconnected");
                return Task.CompletedTask;
            }

            private Task Connection_Closed(Exception arg)
            {
                _logger.LogError($"Consumer connection is closed, details {arg.Message}");
                return Task.CompletedTask;
            }

            public async Task ConnectAsync()
            {
                await _connection.StartAsync().ContinueWith(async task =>
                {
                    if (task.Exception != null)
                    {
                        _logger.LogError($"Consumer failed to connect to Andy X Node, details {task.Exception.Message}");
                        if (_xClientConfiguration.AutoConnect == true)
                        {
                            // retry connection
                            Thread.Sleep(3000);
                            _logger.LogWarning($"Consumer is connectiong to Andy X Node");
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
                        _logger.LogError($"Consumer failed to disconnect from Andy X Node, details {task.Exception.Message}");
                    }
                });
            }
            public async Task AcknowledgeMessage(AcknowledgeMessageArgs acknowledgeMessageArgs)
            {
                await _connection.SendAsync("AcknowledgeMessage", acknowledgeMessageArgs);
            }
        }
    }
}