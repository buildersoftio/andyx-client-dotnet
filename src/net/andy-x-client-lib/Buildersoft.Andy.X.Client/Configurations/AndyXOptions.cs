using Buildersoft.Andy.X.Client.Configurations.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Buildersoft.Andy.X.Client.Configurations
{
    public class AndyXOptions
    {
        public string Uri { get; set; }
        public string Token { get; set; }

        public string Tenant { get; set; }
        public string Product { get; set; }

        public ConnectionStates State { get; set; }

        public AndyXLogger Logger { get; set; }
        public HttpClientHandler HttpClientHandler { get; set; }

        public AndyXOptions()
        {
            State = ConnectionStates.Initialized;
        }

        public static AndyXOptions Create(AndyXOptions andyXOptions)
        {
            return new AndyXOptions()
            {
                Uri = andyXOptions.Uri,
                HttpClientHandler = andyXOptions.HttpClientHandler,
                Logger = andyXOptions.Logger,
                Tenant = andyXOptions.Tenant,
                Product = andyXOptions.Product,
                Token = andyXOptions.Token,
                State = andyXOptions.State
            };
        }
    }
}
