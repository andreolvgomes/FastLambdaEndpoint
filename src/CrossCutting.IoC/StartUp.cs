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
            services.AddScoped<WarmupMiddleware>();
            services.AddScoped<ApiKeyMiddleware>();
            services.AddScoped<LoggingMiddleware>();

            // pipeline
            services.AddScoped(sp =>
            {
                return new MiddlewarePipeline(sp)
                    .Use<WarmupMiddleware>()
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