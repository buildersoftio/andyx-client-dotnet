using Andy.X.Client.Builders;
using Andy.X.Client.Configurations;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Andy.X.Client.Extensions.DependencyInjection
{
    public static class ConsumerDependencyInjectionExtensions
    {
        public static IServiceCollection AddConsumerBuilder<K, V>(this IServiceCollection services, Action<ConsumerConfiguration> configuration)
        {
            ConsumerConfiguration ConsumerConfiguration = new ConsumerConfiguration();
            configuration.Invoke(ConsumerConfiguration);

            var builder = new ConsumerBuilder<K, V>(ConsumerConfiguration);

            return services.AddSingleton<ConsumerBuilder<K, V>>(builder);
        }
    }
}
