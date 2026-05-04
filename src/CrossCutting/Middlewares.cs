using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using FastLambda;
using FastLambda.Middleware;

namespace CrossCutting;

public class ApiKeyMiddleware : ILambdaMiddleware
{
    private readonly RequestContext _context;
    private readonly ICacheService _cacheService;

    public ApiKeyMiddleware(RequestContext context,
        ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<ResponseResult<object>> InvokeAsync(APIGatewayProxyRequest request, ILambdaContext context, Func<Task<ResponseResult<object>>> next)
    {
        //if (request.Headers is null)
        //    return new Unauthorized("API Key inválida");

        //if (!request.Headers.TryGetValue("x-api-key", out var apiKey) || apiKey != "minha-chave")
        //    return new Unauthorized("API Key inválida");

        //_context.ApiKey = apiKey;
        //_context.EmpresaId = empresa.Id;
        //_context.IdDatabase = empresa.Iddatabase;
        //_context.NomeEmpresa = empresa.Nome;

        return await next();
    }
}

public class TenantBootstrapMiddleware : ILambdaMiddleware
{
    private readonly RequestContext _context;
    private readonly ICacheService _cacheService;

    public TenantBootstrapMiddleware(RequestContext context,
        ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<ResponseResult<object>> InvokeAsync(APIGatewayProxyRequest request, ILambdaContext context, Func<Task<ResponseResult<object>>> next)
    {
        //_context.TenantId = tenantId;

        return await next();
    }
}

public class WarmupMiddleware : ILambdaMiddleware
{
    public async Task<ResponseResult<object>> InvokeAsync(APIGatewayProxyRequest request, ILambdaContext context, Func<Task<ResponseResult<object>>> next)
    {
        if (request?.Headers?.TryGetValue("x-warmup", out var value) == true && value == "1")
            return new Success();

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