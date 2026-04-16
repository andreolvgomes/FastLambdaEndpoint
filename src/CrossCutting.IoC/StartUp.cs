using Microsoft.Extensions.DependencyInjection;
using FastLambdaEndpoint.Middleware;
using Infra.Repositories;

namespace CrossCutting.IoC
{
    public class StartUp
    {
        public static IServiceCollection ServiceCollection()
        {
            var services = new ServiceCollection();

            services.AddScoped<IRepository, Repository>();

            // middlewares
            services.AddScoped<ApiKeyMiddleware>();
            services.AddScoped<LoggingMiddleware>();

            // pipeline
            services.AddSingleton(sp =>
            {
                return new MiddlewarePipeline(sp)
                    .Use<LoggingMiddleware>()
                    .Use<ApiKeyMiddleware>();
            });

            return services;
        }

        public static IServiceProvider ServiceProvider()
        {
            return ServiceCollection().BuildServiceProvider();
        }
    }
}