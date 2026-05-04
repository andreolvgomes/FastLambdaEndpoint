using CrossCutting.IoC;
using FastLambda;
using FastLambda.Impl;

namespace Infra.Lambda;

public abstract class Function<THandler, TRequest> : LambdaFunction<THandler, TRequest>
        where THandler : IHandler<TRequest>
        where TRequest : class, new()
{
    protected Function() : base(StartUp.ServiceCollection()) { }
}

public abstract class Function<THandler, TRequest, TResponse> : LambdaFunction<THandler, TRequest, TResponse>
    where THandler : IHandler<TRequest, TResponse>
    where TRequest : class, new()
    where TResponse : class, new()
{
    protected Function() : base(StartUp.ServiceCollection()) { }
}

public class FunctionNoRequest<THandler> : LambdaFunctionNoRequest<THandler>
    where THandler : IHandlerWithoutRequest
{
    protected FunctionNoRequest() : base(StartUp.ServiceCollection()) { }
}

public abstract class FunctionNoRequest<THandler, TResponse> : LambdaFunctionNoRequest<THandler, TResponse>
    where THandler : IHandlerWithoutRequest<TResponse>
    where TResponse : class, new()
{
    protected FunctionNoRequest() : base(StartUp.ServiceCollection()) { }
}

public abstract class FunctionProxy<THandler> : LambdaProxyFunction<THandler>
    where THandler : IHandlerAPIGatewayProxyRequest
{
    protected FunctionProxy() : base(StartUp.ServiceCollection()) { }
}