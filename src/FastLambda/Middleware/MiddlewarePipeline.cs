using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;

namespace FastLambda.Middleware
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
            return await ExecuteStep(0, request, context);
        }

        private async Task<ResponseResult<object>> ExecuteStep(int index, APIGatewayProxyRequest request, ILambdaContext context)
        {
            if (index >= _middlewareTypes.Count)
                return null;

            var type = _middlewareTypes[index];

            var middleware = (ILambdaMiddleware)_serviceProvider.GetRequiredService(type);

            // executa atual e entrega função next()
            return await middleware.InvokeAsync(request, context, 
                () => ExecuteStep(index + 1, request, context));
        }
    }
}