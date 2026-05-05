using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

namespace FastLambda
{
    public interface IHandler<TRequest>
    {
        Task<ResponseResult<Response>> Handler(TRequest request, APIGatewayProxyRequest apiGateway, ILambdaContext context);
    }

    public interface IHandler<TRequest, TResponse>
    {
        Task<ResponseResult<TResponse>> Handler(TRequest request, APIGatewayProxyRequest apiGateway, ILambdaContext context);
    }

    public interface IHandlerNoRequest
    {
        Task<ResponseResult<Response>> Handler(APIGatewayProxyRequest apiGateway, ILambdaContext context);
    }

    public interface IHandlerNoRequest<TResponse>
    {
        Task<ResponseResult<TResponse>> Handler(APIGatewayProxyRequest apiGateway, ILambdaContext context);
    }

    public interface IHandlerProxyRequest
    {
        Task<APIGatewayProxyResponse> Handler(APIGatewayProxyRequest apiGateway, ILambdaContext context);
    }
}