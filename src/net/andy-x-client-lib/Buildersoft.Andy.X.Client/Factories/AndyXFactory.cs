using Buildersoft.Andy.X.Client.Abstraction;
using Buildersoft.Andy.X.Client.Builders;
using Buildersoft.Andy.X.Client.Configurations;
using Buildersoft.Andy.X.Client.Configurations.Logging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Client.Factories
{
    public class AndyXFactory : AndyXBuilder, IAndyXFactory
    {
        public AndyXFactory() : base()
        {

        }

        /// <summary>
        /// Initialize new instance of Andy X Client
        /// </summary>
        /// <example>https://{host}</example>
        /// <param name="url">url of andy x node</param>
        public AndyXFactory(string url) : base(url)
        {

        }

        private AndyXFactory(AndyXOptions andyXOptions, HttpClient client, ILogger logger)
        {
            _andyXOptions = andyXOptions;
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Build Andy X Client
        /// </summary>
        /// <returns>If true client can reach Andy X, if false check parameters in andyxoptions</returns>
        public async Task<IAndyXFactory> BuildAsync()
        {
            _client = new HttpClient(_andyXOptions.HttpClientHandler);

            if (_andyXOptions.Token == "")
            {
                _logger.LogError($"andyx-persistent://errors/description: Token is not provided");
                _andyXOptions.State = ConnectionStates.Failed;
            }
            if (_andyXOptions.Tenant == "")
            {
                _logger.LogError($"andyx-persistent://errors/description: Tenant is not provided");
                _andyXOptions.State = ConnectionStates.Failed;
            }

            _client.DefaultRequestHeaders.Add("x-andy-x-tenant-Authorization", $"Bearer {_andyXOptions.Token}");
            _client.DefaultRequestHeaders.Add("x-andy-x-tenant", _andyXOptions.Tenant);

            string getProductUrl = $"{_andyXOptions.Uri}/api/v1/tenants/{_andyXOptions.Tenant}/products/{_andyXOptions.Product}";
            try
            {
                var response = await _client.GetAsync(getProductUrl);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation($"andyx-persistent://{_andyXOptions.Tenant}: connected");
                    _andyXOptions.State = ConnectionStates.Connected;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"andyx-persistent://{_andyXOptions.Tenant}/errors/details: {e.Message}");
                _andyXOptions.State = ConnectionStates.Failed;
            }

            return this;
        }

        /// <summary>
        /// Get Andy X Client
        /// </summary>
        /// <returns>AndyXClient object</returns>
        public AndyXClient CreateClient()
        {
            return new AndyXFactory(_andyXOptions, _client, _logger) as AndyXClient;
        }
    }
}
