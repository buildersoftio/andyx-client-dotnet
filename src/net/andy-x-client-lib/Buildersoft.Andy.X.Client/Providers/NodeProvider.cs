using Buildersoft.Andy.X.Client.Configurations;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Client
{
    public partial class Reader<TEntity>
    {
        private class NodeProvider
        {
            private readonly HubConnection _connection;
            private readonly AndyXClient _andyXClient;
            private readonly ReaderOptions _readerOptions;

            public NodeProvider(AndyXClient andyXClient, ReaderOptions readerOptions)
            {
                _andyXClient = andyXClient;
                _readerOptions = readerOptions;

                var clientOptions = andyXClient.GetOptions();

                _connection = new HubConnectionBuilder()
                    .WithUrl($"{clientOptions.Uri}/realtime/v1/reader", option =>
                    {
                        option.Headers["Authorization"] = $"Bearer {clientOptions.Token}";
                        option.Headers["x-andy-x-tenant-Authorization"] = $"Bearer {clientOptions.Token}";

                        option.Headers["x-andy-x-tenant"] = clientOptions.Tenant;
                        option.Headers["x-andy-x-product"] = clientOptions.Product;
                        option.Headers["x-andy-x-component"] = _readerOptions.Component;
                        option.Headers["x-andy-x-book"] = _readerOptions.Book;
                        option.Headers["x-andy-x-reader"] = _readerOptions.Name;
                        option.Headers["x-andy-x-readertype"] = _readerOptions.ReaderType.ToString();
                        option.Headers["x-andy-x-readeras"] = _readerOptions.ReaderAs.ToString();
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

            /// <summary>
            /// Get ReaderOptions
            /// </summary>
            /// <returns>ReaderOptions connected to node</returns>
            public ReaderOptions GetReaderOptions()
            {
                return _readerOptions;
            }
        }
    }

}
