using Buildersoft.Andy.X.Client.Configurations;
using Buildersoft.Andy.X.Client.Extensions;
using Buildersoft.Andy.X.Client.Services.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Client
{
    public partial class Reader<TEntity>
    {
        public delegate void OnMessageReceivedHandler(object sender, MessageEventArgs e);
        public event OnMessageReceivedHandler MessageReceived;

        private readonly HttpClient _client;
        private readonly ILogger _logger;

        private readonly AndyXClient _andyXClient;
        private readonly AndyXOptions _andyXOptions;
        private readonly ReaderOptions _readerOptions;
        private NodeReaderService nodeService;

        private bool isBuild;

        public Reader(AndyXClient andyClient)
        {
            _andyXClient = andyClient;
            _readerOptions = new ReaderOptions();

            _andyXOptions = AndyXOptions.Create(andyClient.GetOptions());

            _logger = _andyXOptions
                .Logger
                .GetLoggerFactory()
                .CreateLogger<Reader<TEntity>>();

            _client = new HttpClient(_andyXOptions.HttpClientHandler);
            if (_andyXOptions.Token != "")
                _client.DefaultRequestHeaders.Add("x-andy-x-tenant-Authorization", $"Bearer {_andyXOptions.Token}");

            if (_andyXOptions.Tenant != "")
                _client.DefaultRequestHeaders.Add("x-andy-x-tenant", _andyXOptions.Tenant);

            isBuild = false;
        }

        /// <summary>
        /// Enter book
        /// </summary>
        /// <param name="book">Book name</param>
        /// <returns>reader object</returns>
        public Reader<TEntity> Book(string book)
        {
            _readerOptions.Book = book;
            return this;
        }

        /// <summary>
        /// Enter component
        /// </summary>
        /// <param name="component">Component name</param>
        /// <returns>reader object</returns>
        public Reader<TEntity> Component(string component)
        {
            _readerOptions.Component = component;
            return this;
        }

        /// <summary>
        /// Enter reader name
        /// </summary>
        /// <param name="reader">reader name</param>
        /// <returns>reader object</returns>
        public Reader<TEntity> ReaderName(string reader)
        {
            _readerOptions.Name = reader;
            return this;
        }

        /// <summary>
        /// Enter reader type. Exclusive type allows only this reader to read with same name, Failover type allows only this reader but with a backup, 
        /// Shared type allows multiple readers with same name.
        /// </summary>
        /// <param name="readerType">Reader types enum. Default value is 'Exclusive'</param>
        /// <returns>reader object</returns>
        public Reader<TEntity> ReaderType(ReaderTypes readerType)
        {
            _readerOptions.ReaderType = readerType;
            return this;
        }

        /// <summary>
        /// Enter ReaderAs, Subscription allows reading messages from the moment this reader is connected, Consumer allows to read any message inside a book.
        /// </summary>
        /// <param name="readerAs">ReaderAs type enum. Default value is 'Subscription'</param>
        /// <returns>reader object</returns>
        public Reader<TEntity> ReaderAs(ReaderAs readerAs)
        {
            _readerOptions.ReaderAs = readerAs;
            return this;
        }

        /// <summary>
        /// Build Reader.
        /// This function will create component and book if they do not exists
        /// </summary>
        /// <returns>reader object</returns>
        public Reader<TEntity> Build()
        {
            nodeService = new NodeReaderService(new NodeProvider(_andyXClient, _readerOptions));

            nodeService.ReaderConnected += NodeService_ReaderConnected;
            nodeService.MessageReceived += NodeService_MessageReceived;
            nodeService.ReaderDisconnected += NodeService_ReaderDisconnected;

            string componentRequestUrl = $"{_andyXOptions.Uri}/api/v1/tenants/{_andyXOptions.Tenant}" +
                    $"/products/{_andyXOptions.Product}/components/{_readerOptions.Component}";
            string bookRequestUrl = $"{componentRequestUrl}/books/{_readerOptions.Book}";
            string schemaRequestUrl = $"{bookRequestUrl}/schema?isSchemaValid={_readerOptions.Schema.SchemaValidationStatus}";

            _ = _client.PostAsync(componentRequestUrl, null).Result;

            var body = new StringContent("{}", Encoding.UTF8, "application/json");
            if (_readerOptions.Schema.SchemaValidationStatus == true)
                body = new StringContent(_readerOptions.Schema.Schema, Encoding.UTF8, "application/json");

            var response = _client.GetAsync(bookRequestUrl).Result;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                _client.PostAsync(schemaRequestUrl, body);
                isBuild = true;
            }
            else
            {
                response = _client.PostAsync(bookRequestUrl, body).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                    isBuild = true;
            }

            return this;
        }

        /// <summary>
        /// Build Reader asynchronously.
        /// This function will create component and book if they do not exists
        /// </summary>
        /// <returns>Task of reader instance</returns>
        public Task<Reader<TEntity>> BuildAsync()
        {
            return new Task<Reader<TEntity>>(() =>
            {
                return Build();
            });
        }

        /// <summary>
        /// Connect and start receiving messages
        /// </summary>
        /// <returns></returns>
        public async Task ConnectAsync()
        {
            if (isBuild == true)
                await nodeService.ConnectAsync();

            else
                throw new Exception($"andyx-persistent://{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_readerOptions.Component}/{_readerOptions.Book}/readers/{_readerOptions.Name}: build-failed");
        }

        /// <summary>
        /// Disconnect and stop receiving messages
        /// </summary>
        /// <param name="reason">reason - string</param>
        /// <returns></returns>
        public async Task DisconnectAsync(string reason)
        {
            if (isBuild == true)
                await nodeService.CloseConnectionAsync();
        }

        private void NodeService_ReaderDisconnected(ReaderDisconnectedArgs obj)
        {
            _logger.LogWarning($"andyx-persistent://{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_readerOptions.Component}/{_readerOptions.Book}/readers/{_readerOptions.Name}/{obj.ReaderId}: disconnected");
        }

        private async void NodeService_MessageReceived(MessageReceivedArgs obj)
        {
            _logger.LogInformation($"andyx-persistent://{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_readerOptions.Component}/{_readerOptions.Book}/readers/{_readerOptions.Name}?msgId={obj.MessageId}: received");

            // Trigger MessageReceived with message data
            MessageReceived?.Invoke(this, new MessageEventArgs(obj.Message.ToString().TryJsonToObject<TEntity>()));

            _logger.LogInformation($"andyx-persistent://{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_readerOptions.Component}/{_readerOptions.Book}/readers/{_readerOptions.Name}?msgId={obj.MessageId}: processed");

            await AcknowledgeThisMessage(obj);
        }

        private void NodeService_ReaderConnected(ReaderConnectedArgs obj)
        {
            _logger.LogInformation($"andyx-persistent://{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_readerOptions.Component}/{_readerOptions.Book}/readers/{_readerOptions.Name}/{obj.ReaderId}: connected");
        }

        private async Task AcknowledgeThisMessage(MessageReceivedArgs obj)
        {
            await nodeService.AcknowledgeMessage(new MessageAckDetail()
            {
                MessageId = obj.MessageId,
                Reader = _readerOptions.Name,
                Book = obj.Book,
                Component = obj.Component,
                Product = obj.Product,
                Tenant = obj.Tenant
            });
            _logger.LogInformation($"andyx-persistent://{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_readerOptions.Component}/{_readerOptions.Book}/readers/{_readerOptions.Name}?msgId={obj.MessageId}: acknowledged");
        }
    }
}
