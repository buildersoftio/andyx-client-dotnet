using Andy.X.Client.Configurations;
using Andy.X.Client.Events.Producers;
using Microsoft.AspNetCore.SignalR.Client;
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
            private readonly HubConnection _connection;
            private readonly XClientConfiguration _xClientConfiguration;

            public event Action<ProducerConnectedArgs> ProducerConnected;
            public event Action<ProducerDisconnectedArgs> ProducerDisconnected;
            public event Action<MessageStoredArgs> MessageStored;

            public ProducerNodeService(ProducerNodeProvider producerNodeProvider,
                XClientConfiguration xClientConfiguration)
            {
                _connection = producerNodeProvider.GetHubConnection();
                _xClientConfiguration = xClientConfiguration;

                _connection.On<ProducerConnectedArgs>("ProducerConnected", connectedArgs => ProducerConnected?.Invoke(connectedArgs));
                _connection.On<ProducerDisconnectedArgs>("ProducerDisconnected", disconnected => ProducerDisconnected?.Invoke(disconnected));
                _connection.On<MessageStoredArgs>("MessageStored", received => MessageStored?.Invoke(received));
            }

            public async Task ConnectAsync()
            {
                await _connection.StartAsync().ContinueWith(async task =>
                {
                    if (task.Exception != null)
                    {
                        Console.WriteLine($"Producer connection to Andy X Node failed, details {task.Exception.Message}");
                        if (_xClientConfiguration.AutoConnect == true)
                        {
                            // retry connection
                            Thread.Sleep(3000);
                            Console.WriteLine($"Producer is connectiong to Andy X Node");
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
                        Console.WriteLine($"Producer disconnection from Andy X Node failed, details {task.Exception.Message}");
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
