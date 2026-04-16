using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;

namespace FastLambdaEndpoint.Middleware
{
    public class MiddlewarePipeline
    {
        private readonly IList<Type> _middlewareTypes = new List<Type>();
        private readonly IServiceProvider _serviceProvider;

        public MiddlewarePipeline(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public MiddlewarePipeline Use<TMiddleware>() where TMiddleware : ILambdaMiddleware
        {
            _middlewareTypes.Add(typeof(TMiddleware));
            return this;
        }

        public async Task<ResponseResult<object>> ExecuteAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            Func<Task<ResponseResult<object>>> pipeline = () => Task.FromResult<ResponseResult<object>>(null);

            foreach (var middlewareType in _middlewareTypes)
            {
                var middleware = (ILambdaMiddleware)_serviceProvider.GetRequiredService(middlewareType);

                var next = pipeline;
                pipeline = async () => await middleware.InvokeAsync(request, context, next);
            }

            return await pipeline();
        }
    }
}