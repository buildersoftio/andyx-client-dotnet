using Buildersoft.Andy.X.Client.Configurations;
using Buildersoft.Andy.X.Client.Configurations.Logging;
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
    public class AndyXClient : AndyXFactory
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
    }
}
