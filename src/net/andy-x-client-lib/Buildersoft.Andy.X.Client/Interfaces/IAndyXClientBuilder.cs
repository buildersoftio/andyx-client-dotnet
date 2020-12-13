using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Buildersoft.Andy.X.Client.Interfaces
{
   public interface IAndyXClientBuilder
    {
        IAndyXClientBuilder Url(string url);
        IAndyXClientBuilder Token(string token);
        IAndyXClientBuilder Tenant(string tenant);
        IAndyXClientBuilder Product(string product);
        IAndyXClientBuilder Logger(ILoggerFactory factory);
        IAndyXClientBuilder HttpClientHandler(HttpClientHandler httpClientHandler);

        IAndyXClientBuilder GetAndyXClient();
    }
}
