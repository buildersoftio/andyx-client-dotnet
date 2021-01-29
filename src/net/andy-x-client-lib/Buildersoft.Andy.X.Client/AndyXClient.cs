using Buildersoft.Andy.X.Client.Abstraction;
using Buildersoft.Andy.X.Client.Builders;
using Buildersoft.Andy.X.Client.Configurations;
using Buildersoft.Andy.X.Client.Configurations.Logging;
using Buildersoft.Andy.X.Client.Events;
using Buildersoft.Andy.X.Client.Factories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Client
{
    /// <summary>
    /// connect to andy x node
    /// </summary>
    public class AndyXClient : AndyXBuilder
    {
        public AndyXClient() : base()
        {

        }

        /// <summary>
        /// Initialize new instance of Andy X Client
        /// </summary>
        /// <example>https://{host}</example>
        /// <param name="url">url of andy x node</param>
        public AndyXClient(string url) : base(url)
        {

        }

        /// <summary>
        /// Build Andy X Client
        /// </summary>
        /// <returns>If true client can reach Andy X, if false check parameters in andyxoptions</returns>
        public async Task<AndyXClient> BuildAsync()
        {
            _client = new HttpClient(_andyXOptions.HttpClientHandler);

            if (_andyXOptions.Token == "")
            {
                _logger.LogError($"andyx-persistent://errors/description: Token is not provided");
                _andyXOptions.State = ConnectionStates.Failed;

                Events.OnStateChanged?.Invoke(new StateChangedContext(_andyXOptions.State));
            }
            if (_andyXOptions.Tenant == "")
            {
                _logger.LogError($"andyx-persistent://errors/description: Tenant is not provided");
                _andyXOptions.State = ConnectionStates.Failed;

                Events.OnStateChanged?.Invoke(new StateChangedContext(_andyXOptions.State));
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

                    Events.OnStateChanged?.Invoke(new StateChangedContext(_andyXOptions.State));
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"andyx-persistent://{_andyXOptions.Tenant}/errors/details: {e.Message}");
                _andyXOptions.State = ConnectionStates.Failed;

                Events.OnStateChanged?.Invoke(new StateChangedContext(_andyXOptions.State));
            }

            return this;
        }

    }
}
