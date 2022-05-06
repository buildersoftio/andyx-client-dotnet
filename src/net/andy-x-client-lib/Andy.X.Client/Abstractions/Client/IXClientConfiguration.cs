using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace Andy.X.Client.Abstractions.Client
{
    public interface IXClientConfiguration
    {
       IXClientConfiguration ConfigLogging(ILoggerFactory loggerFactory);
       IXClientConfiguration WithHttpClientHandler(Action<HttpClientHandler> httpHandler);

       XClient Build();
    }
}
