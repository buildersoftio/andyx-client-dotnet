using Buildersoft.Andy.X.Client.Configurations;
using Buildersoft.Andy.X.Client.Configurations.Logging;
using Buildersoft.Andy.X.Client.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace Buildersoft.Andy.X.Client.Builders
{
    public class AndyXBuilder
    {
        protected AndyXOptions _andyXOptions;
        protected HttpClient _client;
        protected ILogger _logger;
        public AndyXEvents Events { get; set; }

        /// <summary>
        /// Initialize new instance of Andy X Client
        /// </summary>
        public AndyXBuilder()
        {
            _andyXOptions = new AndyXOptions() { Logger = new AndyXLogger(), HttpClientHandler = new HttpClientHandler() };
            InitializeLogger();
        }

        private AndyXBuilder(AndyXOptions andyXOptions, HttpClient client, ILogger logger)
        {
            _andyXOptions = andyXOptions;
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Initialize new instance of Andy X Client
        /// </summary>
        /// <example>https://{host}</example>
        /// <param name="url">url of andy x node</param>
        public AndyXBuilder(string url)
        {
            _andyXOptions = new AndyXOptions() { Uri = url, Logger = new AndyXLogger(), HttpClientHandler = new HttpClientHandler() };
            InitializeLogger();
        }

        /// <summary>
        /// Initialize new instance of Andy X Client
        /// </summary>
        /// <example>https://{host}</example>
        /// <param name="url">url of andy x node</param>
        /// <param name="factory">An implementation of ILoggerFactory</param>
        public AndyXBuilder(string url, ILoggerFactory factory)
        {
            _andyXOptions = new AndyXOptions() { Uri = url, Logger = new AndyXLogger(factory), HttpClientHandler = new HttpClientHandler() };
            InitializeLogger();
        }

        /// <summary>
        /// Host of Andy X Node
        /// </summary>
        /// <example>https://{host}</example>
        /// <param name="url">url where andy x node is hosted</param>
        /// <returns>andyxclient instance</returns>
        public AndyXBuilder Url(string url)
        {
            _andyXOptions.Uri = url;
            return this;
        }

        /// <summary>
        /// Tenant Token
        /// </summary>
        /// <param name="token">Jwt Token</param>
        /// <returns>andyxclient instance</returns>
        public AndyXBuilder Token(string token)
        {
            _andyXOptions.Token = token;
            return this;
        }

        /// <summary>
        /// Enter the tenant to conenct
        /// </summary>
        /// <param name="tenant">type string</param>
        /// <returns></returns>
        public AndyXBuilder Tenant(string tenant)
        {
            _andyXOptions.Tenant = tenant;
            return this;
        }

        /// <summary>
        /// Enter the product to connect
        /// </summary>
        /// <param name="product">type string</param>
        /// <returns>andyxclient instance</returns>
        public AndyXBuilder Product(string product)
        {
            _andyXOptions.Product = product;
            return this;
        }

        /// <summary>
        /// Configure logging
        /// </summary>
        /// <param name="factory">an implementation of ILoggerFactory</param>
        /// <returns>andyxclient instance</returns>
        public AndyXBuilder Logger(ILoggerFactory factory)
        {
            _andyXOptions.Logger = new AndyXLogger(factory);
            return this;
        }

        /// <summary>
        /// Implement HttpClientHandler for HttpClient that Andy X Client uses to connect to Andy X Node
        /// </summary>
        /// <param name="httpClientHandler">HttpClientHandler</param>
        /// <returns></returns>
        public AndyXBuilder HttpClientHandler(HttpClientHandler httpClientHandler)
        {
            _andyXOptions.HttpClientHandler = httpClientHandler;
            return this;
        }

        protected AndyXBuilder InitializeBuilder()
        {
            return new AndyXBuilder(_andyXOptions, _client, _logger);
        }


        /// <summary>
        /// Get Andy X Options
        /// </summary>
        /// <returns>andyxOptions</returns>
        public AndyXOptions GetOptions()
        {
            return _andyXOptions;
        }

        private void InitializeLogger()
        {
            _logger = _andyXOptions
                .Logger
                .GetLoggerFactory()
                .CreateLogger<AndyXClient>();
        }
    }

    public class AndyXEvents
    {
        public Action<StateChangedContext> OnStateChanged { get; set; }
    }
}
