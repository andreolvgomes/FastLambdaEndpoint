using Amazon.Lambda.TestUtilities;
using ApiGateway.Lambda.Extensions;
using ApiGateway.Lambda.Utils;
using AWSLambda;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace ApiGateway.Lambda.Endpoints;

[HttpGet("produtos"), AllowAnonymous]
public class GetAllProdutosFunctionEndpoint : EndpointWithoutRequest
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = await new GetAllProdutosFunction().Run(await this.ToProxyRequest(), new TestLambdaContext());
        await Send.ResponseAsync(Json.Deserialize<object>(response.Body), statusCode: response.StatusCode);
    }
}

[HttpPost("produtos"), AllowAnonymous]
public class ProdutosCreateFunctionEndpont : Endpoint<ProdutosRequest>
{
    public async override Task<object> ExecuteAsync(ProdutosRequest req, CancellationToken ct)
    {
        var response = await new ProdutosCreateFunction().Run(await this.ToProxyRequest(req), new TestLambdaContext());
        return await Send.ResponseAsync(Json.Deserialize<object>(response.Body), statusCode: response.StatusCode);
    }
}

[HttpGet("produtos/{id}"), AllowAnonymous]
public class ProdutosGetFunctionEndpoint : EndpointWithoutRequest
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = await new ProdutosGetFunction().Run(await this.ToProxyRequest(), new TestLambdaContext());
        await Send.ResponseAsync(Json.Deserialize<object>(response.Body), statusCode: response.StatusCode);
    }
}