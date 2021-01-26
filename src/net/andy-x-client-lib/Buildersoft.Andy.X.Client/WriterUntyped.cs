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
    public class Writer
    {
        private readonly HttpClient _client;
        private readonly ILogger _logger;
        private readonly AndyXOptions _andyXOptions;
        private readonly WriterOptions _writerOptions;

        public Writer(AndyXClient andyClient)
        {
            _andyXOptions = AndyXOptions.Create(andyClient.GetAndyXOptions());

            _logger = _andyXOptions.Logger.GetLoggerFactory().CreateLogger<Writer>();


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
        public Writer Component(string component)
        {
            _writerOptions.Component = component;
            return this;
        }

        /// <summary>
        /// Enter book
        /// </summary>
        /// <param name="book">book name</param>
        /// <returns>writer instance</returns>
        public Writer Book(string book)
        {
            _writerOptions.Book = book;
            return this;
        }

        /// <summary>
        /// Select file format, default is Json. Data will be transfered by andy x in the selected file format
        /// </summary>
        /// <param name="dataType">DataTypes enum</param>
        /// <returns>writer instance</returns>
        public Writer MessageType(DataTypes dataType)
        {
            _writerOptions.DataType = dataType;
            return this;
        }

        /// <summary>
        /// Configure schema for accepting messages in book.
        /// </summary>
        /// <param name="schemaOptions">Action type SchemaOptions</param>
        /// <returns></returns>
        public Writer Schema(Action<SchemaOptions> schemaOptions)
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
        public Writer WriterType(WriterTypes writerType)
        {
            _writerOptions.WriterType = writerType;
            return this;
        }

        /// <summary>
        /// Build Writer.
        /// This function will create component and book if they do not exists
        /// </summary>
        /// <returns>writer instance</returns>
        public Writer Build()
        {
            string componentRequestUrl = $"{_andyXOptions.Uri}/api/v1/tenants/{_andyXOptions.Tenant}" +
                    $"/products/{_andyXOptions.Product}/components/{_writerOptions.Component}";
            string bookRequestUrl = $"{componentRequestUrl}/books/{_writerOptions.Book}";
            string schemaRequestUrl = $"{bookRequestUrl}/schema?isSchemaValid={_writerOptions.Schema.SchemaValidationStatus}";

            _ = _client.PostAsync(componentRequestUrl, null).Result;

            var bodySchemaReq = new StringContent("{}", Encoding.UTF8, "application/json");
            if (_writerOptions.Schema.SchemaValidationStatus == true)
                bodySchemaReq = new StringContent(_writerOptions.Schema.Schema, Encoding.UTF8, "application/json");

            var response = _client.GetAsync(bookRequestUrl).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _client.PostAsync(schemaRequestUrl, bodySchemaReq);
                return this;
            }

            response = _client.PostAsync(bookRequestUrl, bodySchemaReq).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return this;

            throw new Exception($"andyx-persistent://{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{_writerOptions.Book}/writer: creation failed");
        }

        /// <summary>
        /// Build Writer asynchronously.
        /// This function will create component and book if they do not exists
        /// </summary>
        /// <returns>Task of writer instance</returns>
        public Task<Writer> BuildAsync()
        {
            return new Task<Writer>(() =>
            {
                return Build();
            });
        }

        /// <summary>
        /// Write data object to a book
        /// </summary>
        /// <typeparam name="TEntity">TEntity is a class</typeparam>
        /// <param name="message">Data object to store in a book</param>
        /// <returns>Message id</returns>
        public async Task<Guid> WriteAsync<TEntity>(TEntity message) where TEntity : class
        {
            if (_writerOptions.DataType == DataTypes.Json)
            {
                string jsonMessage = message.ObjectToJson<TEntity>();
                string postUrl = $"{_andyXOptions.Uri}/{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{_writerOptions.Book}";

                var stringContent = new StringContent(jsonMessage, UnicodeEncoding.UTF8, "application/json");

                HttpResponseMessage response = _client.PostAsync(postUrl, stringContent).Result;
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
        /// <typeparam name="TEntity">TEntity is a class</typeparam>
        /// <param name="book">Book name</param>
        /// <param name="message">Data object to store in a book</param>
        /// <param name="checkIfBookExists">Default is true, if this param is true will check always if book exists.</param>
        /// <returns>Message id</returns>
        public async Task<Guid> WriteAsync<TEntity>(string book, TEntity message, bool checkIfBookExists = true) where TEntity : class
        {
            if (_writerOptions.DataType == DataTypes.Json)
            {
                if (checkIfBookExists)
                    TryToCreateWriter(book);

                string jsonMessage = message.ObjectToJson<TEntity>();

                string postUrl = $"{_andyXOptions.Uri}/{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{book}";

                var body = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _client.PostAsync(postUrl, body).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string responseAsString = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"andyx-persistent://{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{book}/messages/{responseAsString.JsonToObject<Guid>()}: sent");
                    return responseAsString.JsonToObject<Guid>();
                }
            }
            _logger.LogError($"andyx-persistent://{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{_writerOptions.Book}/messages: failed");
            return Guid.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity">TEntity is a class</typeparam>
        /// <param name="book">Book name</param>
        /// <param name="msgId">Id of message to store<</param>
        /// <param name="message">Data object to store in a book is TEntity Type</param>
        /// <param name="checkIfBookExists">Default is true, if this param is true will check always if book exists.</param>
        /// <returns></returns>
        public async Task<Guid> WriteAsync<TEntity>(string book, Guid msgId, TEntity message, bool checkIfBookExists = true) where TEntity : class
        {
            Type typeParameterType = typeof(TEntity);
            if (_writerOptions.DataType == DataTypes.Json)
            {
                if (checkIfBookExists)
                    TryToCreateWriter(book);

                string bodyAsJson = message.ObjectToJson<TEntity>();

                string postUrl = $"{_andyXOptions.Uri}/{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{book}?msgId={msgId}";

                var body = new StringContent(bodyAsJson, UnicodeEncoding.UTF8, "application/json");

                var response = await _client.PostAsync(postUrl, body);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string responseAsString = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"andyx-persistent://{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{book}/messages/{responseAsString.JsonToObject<Guid>()}: sent");
                    return responseAsString.JsonToObject<Guid>();
                }
            }
            _logger.LogError($"andyx-persistent://{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{_writerOptions.Book}/messages: failed");

            return Guid.Empty;
        }

        private Writer TryToCreateWriter(string book)
        {
            string requestUrl = $"{_andyXOptions.Uri}/api/v1/tenants/{_andyXOptions.Tenant}" +
                    $"/products/{_andyXOptions.Product}/components/{_writerOptions.Component}/books/{book}";

            var response = _client.GetAsync(requestUrl).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return this;

            var bodySchemaReq = new StringContent("{}", Encoding.UTF8, "application/json");
            if (_writerOptions.Schema.SchemaValidationStatus == true)
                bodySchemaReq = new StringContent(_writerOptions.Schema.Schema, Encoding.UTF8, "application/json");

            response = _client.PostAsync(requestUrl, bodySchemaReq).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return this;

            throw new Exception($"andyx-persistent://{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{_writerOptions.Book}/writer: creation failed");
        }
    }

}
