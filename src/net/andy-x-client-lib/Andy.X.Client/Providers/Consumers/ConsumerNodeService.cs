using Andy.X.Client.Configurations;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Andy.X.Client.Abstractions
{
    public abstract partial class ConsumerBase<T>
    {
        private class ConsumerNodeService
        {
            private readonly HubConnection _connection;
            private readonly XClientConfiguration _xClientConfiguration;

            public event Action<ConsumerConnectedArgs> ConsumerConnected;
            public event Action<ConsumerDisconnectedArgs> ConsumerDisconnected;
            public event Action<MessageInternalReceivedArgs> MessageInternalReceived;

            public ConsumerNodeService(ConsumerNodeProvider consumerNodeProvider,
                XClientConfiguration xClientConfiguration)
            {
                _connection = consumerNodeProvider.GetHubConnection();
                _xClientConfiguration = xClientConfiguration;

                _connection.On<ConsumerConnectedArgs>("ConsumerConnected", connectedArgs => ConsumerConnected?.Invoke(connectedArgs));
                _connection.On<ConsumerDisconnectedArgs>("ConsumerDisconnected", disconnected => ConsumerDisconnected?.Invoke(disconnected));
                _connection.On<MessageInternalReceivedArgs>("MessageSent", received => MessageInternalReceived?.Invoke(received));
            }
            public async Task ConnectAsync()
            {
                await _connection.StartAsync().ContinueWith(async task =>
                {
                    if (task.Exception != null)
                    {
                        Console.WriteLine($"Consumer connection to Andy X Node failed, details {task.Exception.Message}");
                        if (_xClientConfiguration.AutoConnect == true)
                        {
                            // retry connection
                            Thread.Sleep(3000);
                            Console.WriteLine($"Consumer is connectiong to Andy X Node");
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
                        Console.WriteLine($"Consumer disconnection from Andy X Node failed, details {task.Exception.Message}");
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