using System.Net.Http;
using System;
using Microsoft.Extensions.Logging;

namespace Andy.X.Client.Abstractions.XClients
{
    internal interface IXClientConfiguration
    {
        IXClientConfiguration AddLoggingSupport(ILoggerFactory loggerFactory);
        IXClientConfiguration WithHttpClientHandler(Action<HttpClientHandler> httpHandler);

        XClient Build();
    }
}
