using Andy.X.Client.Abstractions;
using Andy.X.Client.Configurations;
using Andy.X.Client.Factories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Andy.X.Client.Extensions.DependencyInjection
{
    public static class AndyXClientDependencyInjectionExtensions
    {
        public static void AddAndyX(this IServiceCollection services, Action<XClientConfiguration> configuration)
        {
            var xClientConfig = new XClientConfiguration();
            configuration.Invoke(xClientConfig);

            services.AddSingleton<IXClientFactory>(provider =>
            {
                return new XClientFactory(xClientConfig);
            });
        }

        public static void UseAndyX(this IApplicationBuilder app)
        {
            IXClientFactory service = app.ApplicationServices.GetService<IXClientFactory>();
            if (service == null)
            {
                throw new Exception("Please add AndyX into ServiceCollection");
            }
        }
    }
}
