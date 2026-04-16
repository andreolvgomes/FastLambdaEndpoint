using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Infra.Lambda;
using Infra.Repositories;
using FastLambdaEndpoint;
using System.ComponentModel.DataAnnotations;

namespace AWSLambda1;

public class ProdutosRequest
{
    [Required]
    public string Name { get; set; }
}
public class ProdutosResponse
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
}

public class ProdutosCreateFunction : Function<ProdutosCreateFunctionHandler, ProdutosRequest, ProdutosResponse> { }
public class ProdutosCreateFunctionHandler : IHandler<ProdutosRequest, ProdutosResponse>
{
    private readonly IRepository _repository;

    public ProdutosCreateFunctionHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResponseResult<ProdutosResponse>> Handler(ProdutosRequest request, APIGatewayProxyRequest apiGateway, ILambdaContext context)
    {
        return new ProdutosResponse()
        {
            Name = request.Name,
        };
    }
}

public class ProdutosGetFunction : FunctionWithoutRequest<ProdutosGetFunctionHandler, ProdutosResponse> { }
public class ProdutosGetFunctionHandler : IHandlerWithoutRequest<ProdutosResponse>
{
    private readonly IRepository _repository;

    public ProdutosGetFunctionHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResponseResult<ProdutosResponse>> Handler(APIGatewayProxyRequest apiGateway, ILambdaContext context)
    {
        return new ProdutosResponse()
        {
            Name = "Monitor LG 29'"
        };
    }
}

public class GetAllProdutosFunction : FunctionWithoutRequest<GetAllProdutosHandler, List<ProdutosResponse>> { }
public class GetAllProdutosHandler : IHandlerWithoutRequest<List<ProdutosResponse>>
{
    private readonly IRepository _repository;

    public GetAllProdutosHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<ResponseResult<List<ProdutosResponse>>> Handler(APIGatewayProxyRequest apiGateway, ILambdaContext context)
    {
        var items = new List<ProdutosResponse>();
        items.Add(new ProdutosResponse() { Name = "Monitor LG 29'" });
        items.Add(new ProdutosResponse() { Name = "Mifa A90" });
        items.Add(new ProdutosResponse() { Name = "Suporte Articulado" });
        return items;
    }
}