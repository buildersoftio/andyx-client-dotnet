using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace Andy.X.Client.Abstractions
{
    public abstract partial class ConsumerBase<T>
    {
        private class ConsumerNodeService
        {
            private readonly HubConnection _connection;

            public event Action<ConsumerConnectedArgs> ConsumerConnected;
            public event Action<ConsumerDisconnectedArgs> ConsumerDisconnected;
            public event Action<MessageInternalReceivedArgs> MessageInternalReceived;

            public ConsumerNodeService(ConsumerNodeProvider consumerNodeProvider)
            {
                _connection = consumerNodeProvider.GetHubConnection();

                _connection.On<ConsumerConnectedArgs>("ConsumerConnected", connectedArgs => ConsumerConnected?.Invoke(connectedArgs));
                _connection.On<ConsumerDisconnectedArgs>("ConsumerDisconnected", disconnected => ConsumerDisconnected?.Invoke(disconnected));
                _connection.On<MessageInternalReceivedArgs>("MessageSent", received => MessageInternalReceived?.Invoke(received));

            }
            public async Task ConnectAsync()
            {
                await _connection.StartAsync().ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        Console.WriteLine($"Consumer connection to Andy X Node failed, details {task.Exception.Message}");
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
        }
    }
}