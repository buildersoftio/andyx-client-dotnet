using Andy.X.Client.Configurations;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace Andy.X.Client
{
    public class XClient
    {
        private readonly XClientConfiguration xClientConfiguration;

        /// <summary>
        /// Initialize XClient object
        /// serviceUrl default value is https://localhost:9001
        /// </summary>
        /// <param name="xClientConfiguration">xClient Configuration</param>
        public XClient(XClientConfiguration xClientConfiguration)
        {
            this.xClientConfiguration = xClientConfiguration;
        }

        /// <summary>
        /// Initialize XClient object
        /// </summary>
        /// <param name="serviceUrl">service url</param>
        public XClient(string serviceUrl)
        {
            xClientConfiguration = new XClientConfiguration(serviceUrl);
        }

        /// <summary>
        /// Tenant Token is needed only if the node asks for it!
        /// </summary>
        /// <param name="tenantToken">Token for Tenant</param>
        /// <returns></returns>
        public XClient TenantToken(string tenantToken)
        {
            xClientConfiguration.TenantToken = tenantToken;
            return this;
        }

        /// <summary>
        /// Tenant where xClient will connect
        /// </summary>
        /// <param name="tenant">Tenant name</param>
        /// <returns>Object of xClient</returns>
        public XClient Tenant(string tenant)
        {
            xClientConfiguration.Tenant = tenant;
            return this;
        }

        /// <summary>
        /// Product where xClient will connect
        /// </summary>
        /// <param name="product">Product name</param>
        /// <returns>Object of xClient</returns>
        public XClient Product(string product)
        {
            xClientConfiguration.Product = product;
            return this;
        }

        /// <summary>
        /// If autoConnect is true, it will retry if the connection is lost.
        /// </summary>
        /// <param name="autoConnect">default value is true</param>
        /// <returns>Object of xClient</returns>
        public XClient AutoConnect(bool autoConnect = true)
        {
            xClientConfiguration.AutoConnect = autoConnect;
            return this;
        }

        /// <summary>
        /// Import logging from your application
        /// </summary>
        /// <param name="loggerFactory">LoggingFactory</param>
        /// <returns>Object of xClient</returns>
        public XClient Logging(ILoggerFactory loggerFactory)
        {
            xClientConfiguration.Logging = new XClientLogging(loggerFactory);
            return this;
        }


        /// <summary>
        /// Import HttpClientHandler configurtion from your applicationj
        /// </summary>
        /// <param name="httpHandler">Action for HttpClientHandler</param>
        /// <returns>Object of xClient></returns>
        public XClient HttpClientHandler(Action<HttpClientHandler> httpHandler)
        {
            httpHandler.Invoke(xClientConfiguration.HttpClientHandler);
            return this;
        }

        /// <summary>
        /// Skip SSL, is should be only used on development environment
        /// </summary>
        /// <returns>Object of xClient</returns>
        public XClient SkipSSLCertificates()
        {
            xClientConfiguration.HttpClientHandler.ServerCertificateCustomValidationCallback +=
                    (sender, certificate, chain, sslPolicyErrors) => { return true; };

            return this;
        }

        /// <summary>
        /// Get ClientConfiguration
        /// </summary>
        /// <returns>Object of XClientConfiguration</returns>
        public XClientConfiguration GetClientConfiguration()
        {
            return xClientConfiguration;
        }
    }
}
