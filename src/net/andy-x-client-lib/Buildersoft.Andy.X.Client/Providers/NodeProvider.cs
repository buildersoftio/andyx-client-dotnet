using Buildersoft.Andy.X.Client.Configurations;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Client
{
    public partial class Reader<TEntity> where TEntity : new()
    {
        private class NodeProvider
        {
            private readonly HubConnection _connection;
            private readonly AndyXClient _andyXClient;

            public NodeProvider(AndyXClient andyXClient)
            {
                _andyXClient = andyXClient;

                var clientOptions = andyXClient.GetOptions();

                _connection = new HubConnectionBuilder()
                    .WithUrl($"{clientOptions.Uri}/realtime/v1/reader", option =>
                    {
                        option.Headers["Authorization"] = $"Bearer {clientOptions.Token}";
                        option.Headers["x-andy-x-tenant-Authorization"] = $"Bearer {clientOptions.Token}";

                        option.Headers["x-andy-x-tenant"] = clientOptions.Tenant;
                        option.Headers["x-andy-x-product"] = clientOptions.Product;
                        option.Headers["x-andy-x-component"] = clientOptions.Component;
                        option.Headers["x-andy-x-book"] = clientOptions.Book;
                        option.Headers["x-andy-x-reader"] = clientOptions.ReaderOptions.Name;
                        option.Headers["x-andy-x-readertype"] = clientOptions.ReaderOptions.ReaderType.ToString();
                        option.Headers["x-andy-x-readeras"] = clientOptions.ReaderOptions.ReaderAs.ToString();
                    })
                    .WithAutomaticReconnect()
                    .Build();
            }

            /// <summary>
            /// Get HubConnection
            /// </summary>
            /// <returns>HubConnections</returns>
            public HubConnection GetHubConnection()
            {
                return _connection;
            }

            /// <summary>
            /// Get AndyXClient
            /// </summary>
            /// <returns>andyxclient object</returns>
            public AndyXClient GetAndyXClient()
            {
                return _andyXClient;
            }
        }
    }

}
