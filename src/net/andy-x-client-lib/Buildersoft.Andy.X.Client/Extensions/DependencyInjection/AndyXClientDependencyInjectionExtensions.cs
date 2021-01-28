using Buildersoft.Andy.X.Client.Builders;
using Buildersoft.Andy.X.Client.Configurations;
using Buildersoft.Andy.X.Client.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Buildersoft.Andy.X.Client.Extensions.DependencyInjection
{
    public static class AndyXClientDependencyInjectionExtensions
    {
        public static void AddAndyX(this IServiceCollection services, Action<AndyXBuilder> builder)
        {
            var andyXBuilder = new AndyXBuilder();

            builder.Invoke(andyXBuilder);
            services.AddSingleton<AndyXFactory>(provder =>
            {
                return andyXBuilder as AndyXFactory;
            });
        }

        public static void AddAndyX(this IServiceCollection services, string url, Action<AndyXBuilder> builder)
        {
            var andyXBuilder = new AndyXBuilder(url);

            builder.Invoke(andyXBuilder);
            services.AddSingleton<AndyXFactory>(provder =>
            {
                return andyXBuilder as AndyXFactory;
            });
        }

        public static void AddAndyX(this IServiceCollection services, string url, ILoggerFactory factory, Action<AndyXBuilder> builder)
        {
            var andyXBuilder = new AndyXBuilder(url, factory);

            builder.Invoke(andyXBuilder);
            services.AddSingleton<AndyXFactory>(provder =>
            {
                return andyXBuilder as AndyXFactory;
            });
        }
    }
}
