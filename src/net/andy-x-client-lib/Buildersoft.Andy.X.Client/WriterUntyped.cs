using Buildersoft.Andy.X.Client.Abstraction;
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
    public class Writer : WriterBase
    {
        public Writer(AndyXClient andyClient) : base(andyClient, typeof(Writer))
        {
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

                var stringContent = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

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

                string messageEndpointPath = $"{_andyXOptions.Uri}/{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{book}";
                string bodyRaw = message.ObjectToJson<TEntity>();


                var body = new StringContent(bodyRaw, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _client.PostAsync(messageEndpointPath, body).Result;
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

                string messageEndpointPath = $"{_andyXOptions.Uri}/{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{book}?msgId={msgId}";
                string bodyRaw = message.ObjectToJson<TEntity>();

                var body = new StringContent(bodyRaw, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(messageEndpointPath, body);
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
            string bookEndpoint = $"{_andyXOptions.Uri}/api/v1/tenants/{_andyXOptions.Tenant}" +
                    $"/products/{_andyXOptions.Product}/components/{_writerOptions.Component}/books/{book}";

            var response = _client.GetAsync(bookEndpoint).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return this;

            var bodyRequest = new StringContent("{}", Encoding.UTF8, "application/json");
            if (_writerOptions.Schema.SchemaValidationStatus == true)
                bodyRequest = new StringContent(_writerOptions.Schema.Schema, Encoding.UTF8, "application/json");

            response = _client.PostAsync(bookEndpoint, bodyRequest).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return this;

            throw new Exception($"andyx-persistent://{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{_writerOptions.Book}/writer: creation failed");
        }
    }

}
