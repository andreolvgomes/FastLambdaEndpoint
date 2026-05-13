using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Dapper;
using FastLambda;
using Infra.Lambda;
using Npgsql;
using System.Text.Json;

namespace AWSLambda
{
    public class FunctionDefault
    {
        public async Task<APIGatewayProxyResponse> Handler(APIGatewayProxyRequest apiGateway, ILambdaContext context)
        {
            var person = new
            {
                name = "André Oliveria Gomes",
                age = 36
            };

            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(person),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }

    public class FunctionDefaultFromDatabase
    {
        public async Task<APIGatewayProxyResponse> Handler(APIGatewayProxyRequest apiGateway, ILambdaContext context)
        {
            var repo = new Repository();
            var item = await repo.GetByIddatabase("ff2f1f46-07aa-4800-9439-041e9ceeb77a");

            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(item),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }

    public class PersonResponse
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class FunctionFastlambdaFunction : FunctionNoRequest<FunctionFastlambdaFunctionHandler, PersonResponse>;
    public class FunctionFastlambdaFunctionHandler : IHandlerNoRequest<PersonResponse>
    {
        public async Task<ResponseResult<PersonResponse>> Handler(APIGatewayProxyRequest apiGateway, ILambdaContext context)
        {
            var person = new PersonResponse
            {
                Name = "André Oliveria Gomes",
                Age = 36
            };

            return person;
        }
    }

    public class FunctionFastlambdaFromDatabaseFunction : FunctionNoRequest<FunctionFastlambdaFromDatabaseFunctionHandler, Tenants>;
    public class FunctionFastlambdaFromDatabaseFunctionHandler : IHandlerNoRequest<Tenants>
    {
        public async Task<ResponseResult<Tenants>> Handler(APIGatewayProxyRequest apiGateway, ILambdaContext context)
        {
            var repo = new Repository();
            var item = await repo.GetByIddatabase("ff2f1f46-07aa-4800-9439-041e9ceeb77a");

            return item;
        }
    }

    public class Tenants
    {
        public long Ten_id { get; set; }
        public string Ten_iddatabase { get; set; }

        public DateTime Created_at { get; set; } = DateTime.UtcNow;
    }

    public class Repository
    {
        public async Task<Tenants> GetByIddatabase(string iddatabase)
        {
            var str = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            using (var _connection = new NpgsqlConnection(str))
            {
                var items = await _connection.QueryAsync<Tenants>("select * from tenants where ten_iddatabase = 'ff2f1f46-07aa-4800-9439-041e9ceeb77a'");
                return items.FirstOrDefault();
            }
        }
    }
}
