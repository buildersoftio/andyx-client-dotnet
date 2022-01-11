using Andy.X.Client.Configurations;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace Andy.X.Client
{
    public class XClient
    {
        private readonly XClientConfiguration xClientConfiguration;

        public XClient(XClientConfiguration xClientConfiguration)
        {
            this.xClientConfiguration = xClientConfiguration;
        }

        public XClient(string serviceUrl)
        {
            xClientConfiguration = new XClientConfiguration(serviceUrl);
        }

        public XClient Tenant(string tenant)
        {
            xClientConfiguration.Tenant = tenant;
            return this;
        }

        public XClient Product(string product)
        {
            xClientConfiguration.Product = product;
            return this;
        }

        public XClient TenantToken(string tenantToken)
        {
            xClientConfiguration.TenantToken =  tenantToken;
            return this;
        }

        public XClient AutoConnect(bool autoConnect)
        {
            xClientConfiguration.AutoConnect = autoConnect;
            return this;
        }

        public XClient Logging(ILoggerFactory loggerFactory)
        {
            xClientConfiguration.Logging = new XClientLogging(loggerFactory);
            return this;
        }

        public XClient HttpClientHandler(Action<HttpClientHandler> httpHandler)
        {
            httpHandler.Invoke(xClientConfiguration.HttpClientHandler);
            return this;
        }

        public XClient SkipSSLCertificates()
        {
            xClientConfiguration.HttpClientHandler.ServerCertificateCustomValidationCallback +=
                    (sender, certificate, chain, sslPolicyErrors) => { return true; };

            return this;
        }

        public XClientConfiguration GetClientConfiguration()
        {
            return xClientConfiguration;
        }
    }
}
