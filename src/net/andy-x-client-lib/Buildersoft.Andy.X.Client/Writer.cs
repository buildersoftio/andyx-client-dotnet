using Buildersoft.Andy.X.Client.Configurations;
using Buildersoft.Andy.X.Client.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Client
{
    public class Writer<TEntity> where TEntity : class
    {
        private readonly HttpClient _client;
        private readonly ILogger _logger;
        private readonly AndyXOptions _andyXOptions;

        public Writer(AndyXClient andyClient)
        {
            _andyXOptions = AndyXOptions.Create(andyClient.GetAndyXOptions());

            _logger = _andyXOptions.Logger.GetLoggerFactory().CreateLogger<Writer<TEntity>>();


            _client = new HttpClient(_andyXOptions.HttpClientHandler);
            if (_andyXOptions.Token != "")
                _client.DefaultRequestHeaders.Add("x-andy-x-tenant-Authorization", $"Bearer {_andyXOptions.Token}");

            if (_andyXOptions.Tenant != "")
                _client.DefaultRequestHeaders.Add("x-andy-x-tenant", _andyXOptions.Tenant);
        }

        /// <summary>
        /// Enter component
        /// </summary>
        /// <param name="component">component name</param>
        /// <returns>writer instance</returns>
        public Writer<TEntity> Component(string component)
        {
            _andyXOptions.Component = component;
            return this;
        }

        /// <summary>
        /// Enter book
        /// </summary>
        /// <param name="book">book name</param>
        /// <returns>writer instance</returns>
        public Writer<TEntity> Book(string book)
        {
            _andyXOptions.Book = book;
            return this;
        }

        /// <summary>
        /// Select file format, default is Json. Data will be transfered by andy x in the selected file format
        /// </summary>
        /// <param name="schemaType">SchemaTypes enum</param>
        /// <returns>writer instance</returns>
        public Writer<TEntity> Schema(SchemaTypes schemaType)
        {
            _andyXOptions.WriterOptions.SchemaType = schemaType;
            return this;
        }

        /// <summary>
        /// Writer type, default is StreamAndStore. StreamAndStore will produce the message to readers and in the same time will be stored into datastorage,
        /// StreamAfterStored will stream message to readers after is stored in datastorage
        /// </summary>
        /// <param name="writerType">Writer type enum</param>
        /// <returns>writer instance</returns>
        public Writer<TEntity> WriterType(WriterTypes writerType)
        {
            _andyXOptions.WriterOptions.WriterType = writerType;
            return this;
        }

        /// <summary>
        /// Build Writer.
        /// This function will create component and book if they do not exists
        /// </summary>
        /// <returns>writer instance</returns>
        public Writer<TEntity> Build()
        {
            string componentRequestUrl = $"{_andyXOptions.Uri}/api/v1/tenants/{_andyXOptions.Tenant}" +
                    $"/products/{_andyXOptions.Product}/components/{_andyXOptions.Component}";

            string bookRequestUrl = $"{_andyXOptions.Uri}/api/v1/tenants/{_andyXOptions.Tenant}" +
                   $"/products/{_andyXOptions.Product}/components/{_andyXOptions.Component}/books/{_andyXOptions.Book}";

            _ = _client.PostAsync(componentRequestUrl, null).Result;

            HttpResponseMessage response = _client.GetAsync(bookRequestUrl).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return this;

            response = _client.PostAsync(bookRequestUrl, null).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return this;

            throw new Exception("Can not create Writer");
        }

        /// <summary>
        /// Build Writer asynchronously.
        /// This function will create component and book if they do not exists
        /// </summary>
        /// <returns>Task of writer instance</returns>
        public Task<Writer<TEntity>> BuildAsync()
        {
            return new Task<Writer<TEntity>>(() =>
            {
                return Build();
            });
        }

        /// <summary>
        /// Write data object to a book
        /// </summary>
        /// <param name="message">Data object to store in a book</param>
        /// <returns>Message id</returns>
        public async Task<Guid> WriteAsync(TEntity message)
        {
            if (_andyXOptions.WriterOptions.SchemaType == SchemaTypes.Json)
            {
                string jsonMessage = message.ObjectToJson<TEntity>();
                string postUrl = $"{_andyXOptions.Uri}/{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_andyXOptions.Component}/{_andyXOptions.Book}";

                var stringContent = new StringContent(jsonMessage, UnicodeEncoding.UTF8, "application/json");

                HttpResponseMessage response = _client.PostAsync(postUrl, stringContent).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string responseAsString = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"andyx-persistent://{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_andyXOptions.Component}/{_andyXOptions.Book}/messages/{responseAsString.JsonToObject<Guid>()}: sent");

                    return responseAsString.JsonToObject<Guid>();
                }
            }
            _logger.LogError($"Message failed to be written");
            return Guid.Empty;
        }

        /// <summary>
        /// Write data object to a book
        /// </summary>
        /// <param name="msgId">Id of message to store</param>
        /// <param name="message">Data object to store in a book</param>
        /// <returns>Message id</returns>
        public async Task<Guid> WriteAsync(Guid msgId, TEntity message)
        {
            if (_andyXOptions.WriterOptions.SchemaType == SchemaTypes.Json)
            {
                string jsonMessage = message.ObjectToJson<TEntity>();

                string postUrl = $"{_andyXOptions.Uri}/{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_andyXOptions.Component}/{_andyXOptions.Book}?msgId={msgId}";

                var stringContent = new StringContent(jsonMessage, UnicodeEncoding.UTF8, "application/json");

                HttpResponseMessage response = _client.PostAsync(postUrl, stringContent).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string responseAsString = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"andyx-persistent://{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_andyXOptions.Component}/{_andyXOptions.Book}/messages/{responseAsString.JsonToObject<Guid>()}: sent");
                    return responseAsString.JsonToObject<Guid>();
                }
            }
            _logger.LogError($"Message failed to be written");
            return Guid.Empty;
        }
    }
}
