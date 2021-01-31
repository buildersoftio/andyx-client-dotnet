using Buildersoft.Andy.X.Client.Abstraction;
using Buildersoft.Andy.X.Client.Configurations;
using Buildersoft.Andy.X.Client.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Client
{
    public class Writer<TEntity> : WriterBase
    {
        public Writer(AndyXClient andyClient) : base(andyClient, typeof(Writer<TEntity>))
        {
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
                string messageEndpointPath = $"{_andyXOptions.Uri}/{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{_writerOptions.Book}";
                string bodyRaw = message.ObjectToJson<TEntity>();
                var body = new StringContent(bodyRaw, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(messageEndpointPath, body);
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
                string messageEndpointPath = $"{_andyXOptions.Uri}/{_andyXOptions.Tenant}/{_andyXOptions.Product}/{_writerOptions.Component}/{_writerOptions.Book}?msgId={msgId}";
                string bodyRaw = message.ObjectToJson<TEntity>();
                var body = new StringContent(bodyRaw, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(messageEndpointPath, body);
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
