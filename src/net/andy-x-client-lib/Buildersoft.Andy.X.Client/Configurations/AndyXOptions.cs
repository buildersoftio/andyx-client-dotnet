using Buildersoft.Andy.X.Client.Configurations.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Buildersoft.Andy.X.Client.Configurations
{
    public class AndyXOptions
    {
        public string Protocol { get; set; }
        public string Uri { get; set; }
        public string Token { get; set; }

        public string Tenant { get; set; }
        public string Product { get; set; }
        public string Component { get; set; }
        public string Book { get; set; }
        public string Reader { get; set; }

        public ReaderOptions ReaderOptions { get; set; }
        public WriterOptions WriterOptions { get; set; }

        public AndyXLogger Logger { get; set; }
        public HttpClientHandler HttpClientHandler { get; set; }

        public AndyXOptions()
        {
            ReaderOptions = new ReaderOptions();
            WriterOptions = new WriterOptions();
        }
    }
}
