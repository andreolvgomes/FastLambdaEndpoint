using CrossCutting.IoC;
using FastLambdaEndpoint;
using FastLambdaEndpoint.Impl;

namespace Infra.Lambda;

public abstract class Function<THandler, TRequest> : FunctionImpl<THandler, TRequest>
        where THandler : IHandler<TRequest>
        where TRequest : class, new()
{
    protected Function() : base(StartUp.ServiceCollection()) { }
}

public abstract class Function<THandler, TRequest, TResponse> : FunctionImpl<THandler, TRequest, TResponse>
    where THandler : IHandler<TRequest, TResponse>
    where TRequest : class, new()
    where TResponse : class, new()
{
    protected Function() : base(StartUp.ServiceCollection()) { }
}

public class FunctionWithoutRequest<THandler> : FunctionWithoutRequestImpl<THandler>
    where THandler : IHandlerWithoutRequest
{
    protected FunctionWithoutRequest() : base(StartUp.ServiceCollection()) { }
}

public abstract class FunctionWithoutRequest<THandler, TResponse> : FunctionWithoutRequestImpl<THandler, TResponse>
    where THandler : IHandlerWithoutRequest<TResponse>
    where TResponse : class, new()
{
    protected FunctionWithoutRequest() : base(StartUp.ServiceCollection()) { }
}