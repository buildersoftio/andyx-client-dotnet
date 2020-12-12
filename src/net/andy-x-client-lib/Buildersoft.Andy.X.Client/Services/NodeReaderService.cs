using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Client
{
    public partial class Reader<TEntity> where TEntity : new()
    {
        private class NodeReaderService
        {
            private readonly HubConnection _connection;

            public event Action<ReaderConnectedArgs> ReaderConnected;
            public event Action<ReaderDisconnectedArgs> ReaderDisconnected;
            public event Action<MessageReceivedArgs> MessageReceived;
        }
    }
}
