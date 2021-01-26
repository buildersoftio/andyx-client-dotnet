using Buildersoft.Andy.X.Client.Configurations;
using Buildersoft.Andy.X.Client.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Buildersoft.Andy.X.Client.Extensions.DependencyInjection
{
    public static class AndyXClientDependencyInjectionExtensions
    {
        public static IAndyXClientBuilder AddAndyX(this IServiceCollection services, Action<IAndyXClientBuilder> builder)
        {
            //TODO... Implement this DI method.
            return new AndyXClient();
        }

        public static TBuilder AddAndyXConfiguration<TBuilder>(this TBuilder builder, Action<AndyXOptions> configure) where TBuilder : IAndyXClientBuilder
        {
            //TODO... Implement this DI method.
            return builder;
        }
    }
}
