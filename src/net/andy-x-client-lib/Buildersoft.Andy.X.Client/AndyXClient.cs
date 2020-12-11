using Buildersoft.Andy.X.Client.Configurations;
using Buildersoft.Andy.X.Client.Configurations.Logging;
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
    public class AndyXClient
    {
        private HttpClient _client;
        private ILogger _logger;
        private AndyXOptions andyXOptions;

        /// <summary>
        /// Initialize new instance of Andy X Client
        /// </summary>
        public AndyXClient()
        {
            andyXOptions = new AndyXOptions() { Logger = new AndyXLogger(), HttpClientHandler = new HttpClientHandler() };
            InitializeLogger();
        }

        /// <summary>
        /// Initialize new instance of Andy X Client
        /// </summary>
        /// <example>https://{host}</example>
        /// <param name="url">url of andy x node</param>
        public AndyXClient(string url)
        {
            andyXOptions = new AndyXOptions() { Uri = url, Logger = new AndyXLogger(), HttpClientHandler = new HttpClientHandler() };
            InitializeLogger();
        }

        /// <summary>
        /// Initialize new instance of Andy X Client
        /// </summary>
        /// <example>https://{host}</example>
        /// <param name="url">url of andy x node</param>
        /// <param name="factory">An implementation of ILoggerFactory</param>
        public AndyXClient(string url, ILoggerFactory factory)
        {
            andyXOptions = new AndyXOptions() { Uri = url, Logger = new AndyXLogger(factory), HttpClientHandler = new HttpClientHandler() };
            InitializeLogger();
        }

        /// <summary>
        /// Host of Andy X Node
        /// </summary>
        /// <example>https://{host}</example>
        /// <param name="url">url where andy x node is hosted</param>
        /// <returns>andyxclient instance</returns>
        public AndyXClient Url(string url)
        {
            andyXOptions.Uri = url;
            return this;
        }

        /// <summary>
        /// Tenant Token
        /// </summary>
        /// <param name="token">Jwt Token</param>
        /// <returns>andyxclient instance</returns>
        public AndyXClient Token(string token)
        {
            andyXOptions.Token = token;
            return this;
        }

        /// <summary>
        /// Enter the tenant to conenct
        /// </summary>
        /// <param name="tenant">type string</param>
        /// <returns></returns>
        public AndyXClient Tenant(string tenant)
        {
            andyXOptions.Tenant = tenant;
            return this;
        }

        /// <summary>
        /// Enter the product to connect
        /// </summary>
        /// <param name="product">type string</param>
        /// <returns>andyxclient instance</returns>
        public AndyXClient Product(string product)
        {
            andyXOptions.Product = product;
            return this;
        }

        /// <summary>
        /// Configure logging
        /// </summary>
        /// <param name="factory">an implementation of ILoggerFactory</param>
        /// <returns>andyxclient instance</returns>
        public AndyXClient Logger(ILoggerFactory factory)
        {
            andyXOptions.Logger = new AndyXLogger(factory);
            return this;
        }

        /// <summary>
        /// Implement HttpClientHandler for HttpClient that Andy X Client uses to connect to Andy X Node
        /// </summary>
        /// <param name="httpClientHandler">HttpClientHandler</param>
        /// <returns></returns>
        public AndyXClient HttpClientHandler(HttpClientHandler httpClientHandler)
        {
            andyXOptions.HttpClientHandler = httpClientHandler;
            return this;
        }

        /// <summary>
        /// Build Andy X Client
        /// </summary>
        /// <returns>If true client can reach Andy X, if false check parameters in andyxoptions</returns>
        public async Task<bool> BuildAsync()
        {
            _client = new HttpClient();
            if (andyXOptions.Token == "")
                _logger.LogError($"Error on building the AndyXClient, error: Provide Token");
            if (andyXOptions.Tenant == "")
                _logger.LogError($"Error on building the AndyXClient, error: Provide Tenant");

            _client.DefaultRequestHeaders.Add("x-andy-x-tenant-Authorization", $"Bearer {andyXOptions.Token}");
            _client.DefaultRequestHeaders.Add("x-andy-x-tenant", andyXOptions.Tenant);

            string getUrl = $"{andyXOptions.Uri}/api/v1/tenants/{andyXOptions.Tenant}/products/{andyXOptions.Product}";
            try
            {
                HttpResponseMessage response = await _client.GetAsync(getUrl);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation($"andyx-persistent://{andyXOptions.Tenant}: connected");
                    return true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error on building the AndyXClient, error description {e.Message}");
            }

            return false;
        }

        /// <summary>
        /// Get Andy X Options
        /// </summary>
        /// <returns>andyxOptions</returns>
        public AndyXOptions GetAndyXOptions()
        {
            return andyXOptions;
        } 

        private void InitializeLogger()
        {
            _logger = andyXOptions
                .Logger
                .GetLoggerFactory()
                .CreateLogger<AndyXClient>();
        }
    }
}
