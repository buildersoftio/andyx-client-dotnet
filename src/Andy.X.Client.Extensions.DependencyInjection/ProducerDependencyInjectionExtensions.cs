using Andy.X.Client.Builders;
using Andy.X.Client.Configurations;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Andy.X.Client.Extensions.DependencyInjection
{
    public static class ProducerDependencyInjectionExtensions
    {
        public static IServiceCollection AddProducerBuilder<K,V>(this IServiceCollection services, Action<ProducerConfiguration> configuration)
        {
            ProducerConfiguration producerConfiguration = new ProducerConfiguration();
            configuration.Invoke(producerConfiguration);

            var builder = new ProducerBuilder<K, V>(producerConfiguration);


            return services.AddSingleton<ProducerBuilder<K, V>>(builder);
        }
    }
}
