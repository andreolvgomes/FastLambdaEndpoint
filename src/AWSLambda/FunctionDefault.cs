using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

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
}
