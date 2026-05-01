using AWSLambda1;
using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFastEndpoints();
//builder.Services.AddFastLambdaLocalHost(typeof(ProdutosCreateFunction).Assembly);

var app = builder.Build();

//app.MapLambdaFunctions(typeof(ProdutosCreateFunction).Assembly);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseFastEndpoints();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

//app.MapPost("/teste", (ProdutosRequest req) =>
//{
//    return Results.Ok(new
//    {
//        ok = true,
//        nome = req.Name
//    });
//})
//.WithName("TestePost")
//.WithTags("Teste");

app.Run();