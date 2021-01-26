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
        private readonly WriterOptions _writerOptions;

        public Writer(AndyXClient andyClient)
        {
            _andyXOptions = AndyXOptions.Create(andyClient.GetAndyXOptions());
            _writerOptions = new WriterOptions();

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
            _writerOptions.Component = component;
            return this;
        }

        /// <summary>
        /// Enter book
        /// </summary>
        /// <param name="book">book name</param>
        /// <returns>writer instance</returns>
        public Writer<TEntity> Book(string book)
        {
            _writerOptions.Book = book;
            return this;
        }

        /// <summary>
        /// Select file format, default is Json. Data will be transfered by andy x in the selected file format
        /// </summary>
        /// <param name="dataType">DataTypes enum</param>
        /// <returns>writer instance</returns>
        public Writer<TEntity> MessageType(DataTypes dataType)
        {
            _writerOptions.DataType = dataType;
            return this;
        }

        /// <summary>
        /// Configure schema for accepting messages in book.
        /// </summary>
        /// <param name="schemaOptions">Action type SchemaOptions</param>
        /// <returns></returns>
        public Writer<TEntity> Schema(Action<SchemaOptions> schemaOptions)
        {
            schemaOptions.Invoke(_writerOptions.Schema);
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
            _writerOptions.WriterType = writerType;
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
                    $"/products/{_andyXOptions.Product}/components/{_writerOptions.Component}";
            string bookRequestUrl = $"{componentRequestUrl}/books/{_writerOptions.Book}";
            string schemaRequestUrl = $"{bookRequestUrl}/schema?isSchemaValid={_writerOptions.Schema.SchemaValidationStatus}";

            _ = _client.PostAsync(componentRequestUrl, null).Result;

            var body = new StringContent("{}", UnicodeEncoding.UTF8, "application/json");
            if (_writerOptions.Schema.SchemaValidationStatus == true)
                body = new StringContent(_writerOptions.Schema.Schema, UnicodeEncoding.UTF8, "application/json");

            var response = _client.GetAsync(bookRequestUrl).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _client.PostAsync(schemaRequestUrl, body);
                return this;
            }

            response = _client.PostAsync(bookRequestUrl, body).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return this;

            throw new Exception($"andyx-persistent://{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{_writerOptions.Book}/writer: creation failed");
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
            if (_writerOptions.DataType == DataTypes.Json)
            {
                string jsonMessage = message.ObjectToJson<TEntity>();
                string postUrl = $"{_andyXOptions.Uri}/{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{_writerOptions.Book}";
                var body = new StringContent(jsonMessage, UnicodeEncoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync(postUrl, body);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string responseAsString = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"andyx-persistent://{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{_writerOptions.Book}/messages/{responseAsString.JsonToObject<Guid>()}: sent");

                    return responseAsString.JsonToObject<Guid>();
                }
            }

            _logger.LogError($"andyx-persistent://{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{_writerOptions.Book}/messages: failed");
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
            if (_writerOptions.DataType == DataTypes.Json)
            {
                string jsonMessage = message.ObjectToJson<TEntity>();
                string postUrl = $"{_andyXOptions.Uri}/{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{_writerOptions.Book}?msgId={msgId}";
                var body = new StringContent(jsonMessage, UnicodeEncoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync(postUrl, body);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string responseAsString = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"andyx-persistent://{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{_writerOptions.Book}/messages/{responseAsString.JsonToObject<Guid>()}: sent");
                    return responseAsString.JsonToObject<Guid>();
                }
            }

            _logger.LogError($"andyx-persistent://{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{_writerOptions.Book}/messages: failed");
            return Guid.Empty;
        }
    }
}
