using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using FastLambda.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace FastLambda.Impl;

public abstract class LambdaFunction<THandler, TRequest> : LambdaFunctionBase
    where THandler : IHandler<TRequest>
{
    protected LambdaFunction(IServiceCollection serviceCollection)
    {
        BuildServiceProvider(typeof(THandler), serviceCollection);
    }

    public async Task<APIGatewayProxyResponse> Run(APIGatewayProxyRequest apiGateway, ILambdaContext context)
    {
        using (var scope = CreateScope())
        {
            var middlewareResponse = await RunMiddleware(scope.ServiceProvider, apiGateway, context);
            if (middlewareResponse != null)
                return ActionResult(middlewareResponse);

            var request = DeserializeObject<TRequest>(apiGateway.Body);

            var errors = TryValidateObject(request);
            if (errors.Any())
                return BadRequest(errors.Select(e => e.MemberNames.FirstOrDefault() + ": " + e.ErrorMessage).ToList());

            var func = scope.ServiceProvider.GetRequiredService<THandler>();
            var response = await func.Handler(request, apiGateway, context);

            return ActionResult(response);
        }
    }
}

public abstract class LambdaFunction<THandler, TRequest, TResponse> : LambdaFunctionBase
    where THandler : IHandler<TRequest, TResponse>
{
    protected LambdaFunction(IServiceCollection serviceCollection)
    {
        BuildServiceProvider(typeof(THandler), serviceCollection);
    }

    public async Task<APIGatewayProxyResponse> Run(APIGatewayProxyRequest apiGateway, ILambdaContext context)
    {
        using (var scope = CreateScope())
        {
            var middlewareResponse = await RunMiddleware(scope.ServiceProvider, apiGateway, context);
            if (middlewareResponse != null)
                return ActionResult(middlewareResponse);

            var request = DeserializeObject<TRequest>(apiGateway.Body);

            var errors = TryValidateObject(request);
            if (errors.Any())
                return BadRequest(errors.Select(e => e.MemberNames.FirstOrDefault() + ": " + e.ErrorMessage).ToList());

            var func = scope.ServiceProvider.GetRequiredService<THandler>();
            var response = await func.Handler(request, apiGateway, context);

            return ActionResult(response);
        }
    }
}

public abstract class LambdaFunctionNoRequest<THandler> : LambdaFunctionBase
    where THandler : IHandlerNoRequest
{
    protected LambdaFunctionNoRequest(IServiceCollection serviceCollection)
    {
        BuildServiceProvider(typeof(THandler), serviceCollection);
    }

    public async Task<APIGatewayProxyResponse> Run(APIGatewayProxyRequest apiGateway, ILambdaContext context)
    {
        using (var scope = CreateScope())
        {
            var middlewareResponse = await RunMiddleware(scope.ServiceProvider, apiGateway, context);
            if (middlewareResponse != null)
                return ActionResult(middlewareResponse);

            var func = scope.ServiceProvider.GetRequiredService<THandler>();
            var response = await func.Handler(apiGateway, context);

            return ActionResult(response);
        }
    }
}

public abstract class LambdaFunctionNoRequest<THandler, TResponse> : LambdaFunctionBase
    where THandler : IHandlerNoRequest<TResponse>
{
    protected LambdaFunctionNoRequest(IServiceCollection serviceCollection)
    {
        BuildServiceProvider(typeof(THandler), serviceCollection);
    }

    public async Task<APIGatewayProxyResponse> Run(APIGatewayProxyRequest apiGateway, ILambdaContext context)
    {
        using (var scope = CreateScope())
        {
            var middlewareResponse = await RunMiddleware(scope.ServiceProvider, apiGateway, context);
            if (middlewareResponse != null)
                return ActionResult(middlewareResponse);

            var func = scope.ServiceProvider.GetRequiredService<THandler>();
            var response = await func.Handler(apiGateway, context);

            return ActionResult(response);
        }
    }
}

public abstract class LambdaFunctionProxyRequest<THandler> : LambdaFunctionBase
    where THandler : IHandlerProxyRequest
{
    protected LambdaFunctionProxyRequest(IServiceCollection serviceCollection)
    {
        BuildServiceProvider(typeof(THandler), serviceCollection);
    }

    public async Task<APIGatewayProxyResponse> Run(APIGatewayProxyRequest apiGateway, ILambdaContext context)
    {
        using (var scope = CreateScope())
        {
            var middlewareResponse = await RunMiddleware(scope.ServiceProvider, apiGateway, context);
            if (middlewareResponse != null)
                return ActionResult(middlewareResponse);

            var func = scope.ServiceProvider.GetRequiredService<THandler>();
            return await func.Handler(apiGateway, context);
        }
    }
}

public abstract class LambdaFunctionBase
{
    protected static IServiceProvider _serviceProvider;
    /// <summary>
    /// Para teste local com ApiGateway
    /// </summary>
    private static readonly ConcurrentDictionary<Type, IServiceProvider> _providers = new();

    public void BuildServiceProvider(Type functionImpl, IServiceCollection serviceCollection)
    {
        _serviceProvider = _providers.GetOrAdd(functionImpl, _ =>
        {
            serviceCollection.AddScoped(functionImpl);
            return serviceCollection.BuildServiceProvider();
        });
    }

    public APIGatewayProxyResponse ActionResult<T>(ResponseResult<T> result)
    {
        if (result.HttpStatus is Success) return Ok();
        if (result.HttpStatus is Created) return Created();
        if (result.HttpStatus is Deleted) return NoContent();
        if (result.HttpStatus is Updated) return NoContent();

        if (result.HttpStatus is NotFound) return Errors(result.Errors, httpStatusCode: HttpStatusCode.NotFound);
        if (result.HttpStatus is BadRequest) return Errors(result.Errors, httpStatusCode: HttpStatusCode.BadRequest);
        if (result.HttpStatus is Unauthorized) return Errors(result.Errors, httpStatusCode: HttpStatusCode.Unauthorized);

        if (result.Errors.Count > 0)
            return BadRequest(result.Errors);

        return Ok(result.Data);
    }

    protected IServiceScope CreateScope()
    {
        var scope = _serviceProvider.CreateScope();
        return scope;
    }

    protected async Task<ResponseResult<object>> RunMiddleware(IServiceProvider serviceProvider, APIGatewayProxyRequest apiGateway, ILambdaContext context)
    {
        var pipeline = serviceProvider.GetService<MiddlewarePipeline>();
        if (pipeline is null)
            return null;

        return await pipeline.ExecuteAsync(apiGateway, context);
    }

    protected APIGatewayProxyResponse Ok(object value = null)
    {
        return new()
        {
            Body = value != null ? SerializeObject(value) : null,
            StatusCode = (int)HttpStatusCode.OK,
            Headers = new Dictionary<string, string>() { ["Content-Type"] = "application/json" }
        };
    }

    protected APIGatewayProxyResponse BadRequest(string message)
    {
        return BadRequest(new List<string> { message });
    }

    protected APIGatewayProxyResponse BadRequest(List<string> messagesErrors, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new()
        {
            Body = SerializeObject(new { messages = messagesErrors }),
            StatusCode = (int)statusCode,
            Headers = new Dictionary<string, string>() { ["Content-Type"] = "application/json" }
        };
    }

    protected APIGatewayProxyResponse NoContent()
    {
        return Response(null, HttpStatusCode.NoContent);
    }

    protected APIGatewayProxyResponse Created()
    {
        return Response(null, HttpStatusCode.Created);
    }

    protected APIGatewayProxyResponse Response(object data, HttpStatusCode httpStatusCode = HttpStatusCode.NoContent)
    {
        return new()
        {
            Body = data != null ? SerializeObject(data) : null,
            StatusCode = (int)httpStatusCode,
            Headers = new Dictionary<string, string>() { ["Content-Type"] = "application/json" }
        };
    }

    protected APIGatewayProxyResponse Errors(List<string> messagesErrors, HttpStatusCode httpStatusCode = HttpStatusCode.NoContent)
    {
        return new()
        {
            Body = SerializeObject(new { messages = messagesErrors }),
            StatusCode = (int)httpStatusCode,
            Headers = new Dictionary<string, string>() { ["Content-Type"] = "application/json" }
        };
    }

    protected List<ValidationResult> TryValidateObject<T>(T obj)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(obj, null, null);
        Validator.TryValidateObject(obj, context, results, true);
        return results;
    }

    protected string SerializeObject(object value)
    {
        var camelSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        return JsonConvert.SerializeObject(value, settings: camelSettings);
    }

    protected T DeserializeObject<T>(string value)
    {
        var camelSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        return JsonConvert.DeserializeObject<T>(value, settings: camelSettings);
    }
}
