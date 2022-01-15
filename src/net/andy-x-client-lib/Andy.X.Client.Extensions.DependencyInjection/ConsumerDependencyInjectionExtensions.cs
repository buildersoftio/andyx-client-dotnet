using Andy.X.Client.Builders;
using Andy.X.Client.Configurations;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Andy.X.Client.Extensions.DependencyInjection
{
    public static class ConsumerDependencyInjectionExtensions
    {
        public static IServiceCollection AddConsumerBuilder<T>(this IServiceCollection services, Action<ConsumerConfiguration<T>> configuration)
        {
            ConsumerConfiguration<T> ConsumerConfiguration = new ConsumerConfiguration<T>();
            configuration.Invoke(ConsumerConfiguration);

            var builder = new ConsumerBuilder<T>(ConsumerConfiguration);

            return services.AddSingleton<ConsumerBuilder<T>>(builder);
        }
    }
}
