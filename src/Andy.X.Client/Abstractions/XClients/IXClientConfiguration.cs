using System;
using Andy.X.Client.Configurations;

namespace Andy.X.Client.Abstractions.XClients
{
    public interface IXClientConfiguration
    {
        IXClientConfiguration WithSettings(Action<XClientSettings> settings);
        XClient Build();
    }
}
