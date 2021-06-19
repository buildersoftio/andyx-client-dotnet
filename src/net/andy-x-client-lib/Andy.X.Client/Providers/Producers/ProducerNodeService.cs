using Andy.X.Client.Events.Producers;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace Andy.X.Client.Abstractions
{
    public abstract partial class ProducerBase<T>
    {
        private class ProducerNodeService
        {
            private readonly HubConnection _connection;

            public event Action<ProducerConnectedArgs> ProducerConnected;
            public event Action<ProducerDisconnectedArgs> ProducerDisconnected;
            public event Action<MessageStoredArgs> MessageStored;

            public ProducerNodeService(ProducerNodeProvider producerNodeProvider)
            {
                _connection = producerNodeProvider.GetHubConnection();

                _connection.On<ProducerConnectedArgs>("ProducerConnected", connectedArgs => ProducerConnected?.Invoke(connectedArgs));
                _connection.On<ProducerDisconnectedArgs>("ProducerDisconnected", disconnected => ProducerDisconnected?.Invoke(disconnected));
                _connection.On<MessageStoredArgs>("MessageStored", received => MessageStored?.Invoke(received));
            }
            public async Task ConnectAsync()
            {
                await _connection.StartAsync().ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        Console.WriteLine($"Producer connection to Andy X Node failed, details {task.Exception.Message}");
                    }
                });
            }

            /// <summary>
            /// Disconnect from Andy X Node
            /// </summary>
            /// <returns></returns>
            public async Task CloseConnectionAsync()
            {
                await _connection.StopAsync().ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        Console.WriteLine($"Producer disconnection from Andy X Node failed, details {task.Exception.Message}");
                    }
                });
            }

            public async Task TransmitMessage(TransmitMessageArgs transmitMessageArgs)
            {
                await _connection.SendAsync("TransmitMessage", transmitMessageArgs);
            }
        }
    }
}
