using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

namespace FastLambdaEndpoint
{
    public interface IHandler<TRequest>
        where TRequest : class, new()
    {
        Task<ResponseResult<Response>> Handler(TRequest request, APIGatewayProxyRequest apiGateway, ILambdaContext context);
    }

    public interface IHandler<TRequest, TResponse>
        where TResponse : class, new()
    {
        Task<ResponseResult<TResponse>> Handler(TRequest request, APIGatewayProxyRequest apiGateway, ILambdaContext context);
    }

    public interface IHandlerWithoutRequest
    {
        Task<ResponseResult<Response>> Handler(APIGatewayProxyRequest apiGateway, ILambdaContext context);
    }

    public interface IHandlerWithoutRequest<TResponse>
        where TResponse : class, new()
    {
        Task<ResponseResult<TResponse>> Handler(APIGatewayProxyRequest apiGateway, ILambdaContext context);
    }
}
