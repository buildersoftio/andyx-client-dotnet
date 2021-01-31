using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Client
{
    public partial class Reader<TEntity>
    {
        private class NodeReaderService
        {
            private readonly HubConnection _connection;

            public event Action<ReaderConnectedArgs> ReaderConnected;
            public event Action<ReaderDisconnectedArgs> ReaderDisconnected;
            public event Action<MessageReceivedArgs> MessageReceived;

            private readonly ILogger _logger;

            public NodeReaderService(NodeProvider nodeProvider)
            {
                _logger = nodeProvider
                    .GetAndyXClient()
                    .GetOptions()
                    .Logger
                    .GetLoggerFactory()
                    .CreateLogger<NodeReaderService>();

                _connection = nodeProvider.GetHubConnection();

                _connection.On<ReaderConnectedArgs>("ReaderConnected", connectedArgs => ReaderConnected?.Invoke(connectedArgs));
                _connection.On<ReaderDisconnectedArgs>("ReaderDisconnected", disconnected => ReaderDisconnected?.Invoke(disconnected));
                _connection.On<MessageReceivedArgs>("MessageReceived", received => MessageReceived?.Invoke(received));
            }

            /// <summary>
            /// Connect to Andy X Node
            /// </summary>
            /// <returns></returns>
            public async Task ConnectAsync()
            {
                await _connection.StartAsync().ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        _logger.LogError($"Connection to Andy X Node failed, details {task.Exception.Message}");
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
                        _logger.LogError($"Diconnection from Andy X Node failed, details {task.Exception.Message}");
                    }
                });
            }

            /// <summary>
            /// Send to Andy X Node - Message as acknowledged
            /// </summary>
            /// <param name="message">MessageAckDetail type</param>
            /// <returns></returns>
            public async Task AcknowledgeMessage(MessageAckDetail message)
            {
                await _connection.SendAsync("AcknowledgeMessage", message);
            }
        }
    }
}
