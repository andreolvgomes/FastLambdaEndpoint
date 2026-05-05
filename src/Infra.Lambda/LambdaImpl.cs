using CrossCutting.IoC;
using FastLambda;
using FastLambda.Impl;

namespace Infra.Lambda;

public abstract class Function<THandler, TRequest> : LambdaFunction<THandler, TRequest>
        where THandler : IHandler<TRequest>
{
    protected Function() : base(StartUp.ServiceCollection()) { }
}

public abstract class Function<THandler, TRequest, TResponse> : LambdaFunction<THandler, TRequest, TResponse>
    where THandler : IHandler<TRequest, TResponse>
{
    protected Function() : base(StartUp.ServiceCollection()) { }
}

public class FunctionNoRequest<THandler> : LambdaFunctionNoRequest<THandler>
    where THandler : IHandlerNoRequest
{
    protected FunctionNoRequest() : base(StartUp.ServiceCollection()) { }
}

public abstract class FunctionNoRequest<THandler, TResponse> : LambdaFunctionNoRequest<THandler, TResponse>
    where THandler : IHandlerNoRequest<TResponse>
{
    protected FunctionNoRequest() : base(StartUp.ServiceCollection()) { }
}

public abstract class FunctionProxyRequest<THandler> : LambdaFunctionProxyRequest<THandler>
    where THandler : IHandlerProxyRequest
{
    protected FunctionProxyRequest() : base(StartUp.ServiceCollection()) { }
}