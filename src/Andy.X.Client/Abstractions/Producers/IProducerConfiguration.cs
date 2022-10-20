using Andy.X.Client.Configurations;
using System;
using System.Collections.Generic;

namespace Andy.X.Client.Abstractions.Producers
{
    public interface IProducerConfiguration<K,V>
    {
        IProducerConfiguration<K,V> AddDefaultHeader(string key, string value);
        IProducerConfiguration<K,V> AddDefaultHeaders(IDictionary<string, string> headers);

        IProducerConfiguration<K,V> WithSettings(Action<ProducerSettings> settings);

        Producer<K,V> Build();
    }
}