using Buildersoft.Andy.X.Client.Configurations;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Client.Abstraction
{
    public abstract class WriterBase
    {
        protected readonly HttpClient _client;
        protected readonly ILogger _logger;
        protected readonly AndyXOptions _andyXOptions;
        protected readonly WriterOptions _writerOptions;

        public WriterBase(AndyXClient andyClient, Type type)
        {
            _andyXOptions = AndyXOptions.Create(andyClient.GetOptions());
            _writerOptions = new WriterOptions();

            _logger = _andyXOptions.Logger.GetLoggerFactory().CreateLogger(type);

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
        public WriterBase Component(string component)
        {
            _writerOptions.Component = component;
            return this;
        }

        /// <summary>
        /// Enter book
        /// </summary>
        /// <param name="book">book name</param>
        /// <returns>writer instance</returns>
        public WriterBase Book(string book)
        {
            _writerOptions.Book = book;
            return this;
        }

        /// <summary>
        /// Select file format, default is Json. Data will be transfered by andy x in the selected file format
        /// </summary>
        /// <param name="dataType">DataTypes enum</param>
        /// <returns>writer instance</returns>
        public WriterBase MessageType(DataTypes dataType)
        {
            _writerOptions.DataType = dataType;
            return this;
        }

        /// <summary>
        /// Configure schema for accepting messages in book.
        /// </summary>
        /// <param name="schemaOptions">Action type SchemaOptions</param>
        /// <returns></returns>
        public WriterBase Schema(Action<SchemaOptions> schemaOptions)
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
        public WriterBase WriterType(WriterTypes writerType)
        {
            _writerOptions.WriterType = writerType;
            return this;
        }

        /// <summary>
        /// Build Writer Asynchronously
        /// This function will create component and book if they do not exists
        /// </summary>
        /// <returns>writer instance</returns>
        public async Task<WriterBase> BuildAsync()
        {
            string componentEndpoint = $"{_andyXOptions.Uri}/api/v1/tenants/{_andyXOptions.Tenant}" +
                    $"/products/{_andyXOptions.Product}/components/{_writerOptions.Component}";
            string bookEndpoint = $"{componentEndpoint}/books/{_writerOptions.Book}";
            string schemaEndpoint = $"{bookEndpoint}/schema?isSchemaValid={_writerOptions.Schema.SchemaValidationStatus}";

            await _client.PostAsync(componentEndpoint, null);

            var bodyRequest = new StringContent("{}", Encoding.UTF8, "application/json");
            if (_writerOptions.Schema.SchemaValidationStatus == true)
                bodyRequest = new StringContent(_writerOptions.Schema.Schema, Encoding.UTF8, "application/json");

            var response = await _client.GetAsync(bookEndpoint);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                await _client.PostAsync(schemaEndpoint, bodyRequest);
                return this;
            }

            response = await _client.PostAsync(bookEndpoint, bodyRequest);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return this;

            throw new Exception($"andyx-persistent://{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{_writerOptions.Book}/writer: creation failed");
        }

        /// <summary>
        /// Build Writer
        /// This function will create component and book if they do not exists
        /// </summary>
        /// <returns>Task of writer instance</returns>
        public WriterBase Build()
        {
            return BuildAsync().Result;
        }
    }
}
