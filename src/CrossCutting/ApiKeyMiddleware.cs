using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using FastLambdaEndpoint;
using FastLambdaEndpoint.Middleware;

namespace CrossCutting;

public class ApiKeyMiddleware : ILambdaMiddleware
{
    public async Task<ResponseResult<object>> InvokeAsync(APIGatewayProxyRequest request, ILambdaContext context, Func<Task<ResponseResult<object>>> next)
    {
        //if (request.Headers is null)
        //    return new Unauthorized("API Key inválida");

        //if (!request.Headers.TryGetValue("x-api-key", out var apiKey) || apiKey != "minha-chave")
        //    return new Unauthorized("API Key inválida");

        return await next();
    }
}

public class LoggingMiddleware : ILambdaMiddleware
{
    public async Task<ResponseResult<object>> InvokeAsync(APIGatewayProxyRequest request, ILambdaContext context, Func<Task<ResponseResult<object>>> next)
    {
        Console.WriteLine($"[Request] {request.HttpMethod} {request.Path}");
        Console.WriteLine("[Response] concluído");

        return await next();
    }
}