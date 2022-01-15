using Andy.X.Client.Builders;
using Andy.X.Client.Configurations;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Andy.X.Client.Extensions.DependencyInjection
{
    public static class ProducerDependencyInjectionExtensions
    {
        public static IServiceCollection AddProducerBuilder<T>(this IServiceCollection services, Action<ProducerConfiguration<T>> configuration)
        {
            ProducerConfiguration<T> producerConfiguration = new ProducerConfiguration<T>();
            configuration.Invoke(producerConfiguration);

            var builder = new ProducerBuilder<T>(producerConfiguration);

            return services.AddSingleton<ProducerBuilder<T>>(builder);
        }
    }
}
