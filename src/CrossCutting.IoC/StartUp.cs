using Microsoft.Extensions.DependencyInjection;
using FastLambda.Middleware;
using Infra.Repositories;

namespace CrossCutting.IoC
{
    public class StartUp
    {
        public static IServiceCollection ServiceCollection()
        {
            var services = new ServiceCollection();

            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();

            services.AddScoped<IRepository, Repository>();
            services.AddScoped<RequestContext>();

            // middlewares
            services.AddScoped<WarmupMiddleware>();
            services.AddScoped<ApiKeyMiddleware>();
            services.AddScoped<LoggingMiddleware>();

            // pipeline
            services.AddScoped(sp =>
            {
                return new MiddlewarePipeline(sp)
                    .Use<WarmupMiddleware>()
                    .Use<ApiKeyMiddleware>()
                    .Use<LoggingMiddleware>();
            });

            return services;
        }

        public static IServiceProvider ServiceProvider()
        {
            return ServiceCollection().BuildServiceProvider();
        }
    }
}