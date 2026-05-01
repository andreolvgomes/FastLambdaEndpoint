using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using System.Reflection;

//[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
//public class LambdaRouteAttribute : Attribute
//{
//    public string Method { get; }
//    public string Route { get; }

//    public LambdaRouteAttribute(string method, string route)
//    {
//        Method = method.ToUpper();
//        Route = route;
//    }
//}

public static class FastLambdaLocalHostExtensions
{
    public static IServiceCollection AddFastLambdaLocalHost(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
            assemblies = new[] { Assembly.GetCallingAssembly() };

        var allTypes = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => x.GetCustomAttribute<LambdaRouteAttribute>() != null)
            .ToList();

        foreach (var type in allTypes)
            services.AddTransient(type);

        return services;
    }

    public static void MapLambdaFunctions(this WebApplication app, params Assembly[] assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
            assemblies = new[] { Assembly.GetCallingAssembly() };

        var allTypes = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => x.GetCustomAttribute<LambdaRouteAttribute>() != null)
            .ToList();

        foreach (var type in allTypes)
        {
            var route = type.GetCustomAttribute<LambdaRouteAttribute>()!;

            app.MapMethods(route.Route, new[] { route.Method }, async context =>
            {
                var instance = app.Services.GetRequiredService(type);

                var proxyRequest = await context.ToProxyRequest();
                var lambdaContext = new TestLambdaContext();

                dynamic handler = instance;
                APIGatewayProxyResponse result = await handler.Run(proxyRequest, lambdaContext);

                context.Response.StatusCode = result.StatusCode;

                if (result.Headers != null)
                {
                    foreach (var h in result.Headers)
                        context.Response.Headers[h.Key] = h.Value;
                }

                if (!string.IsNullOrWhiteSpace(result.Body))
                    await context.Response.WriteAsync(result.Body);
            })
            .WithName(type.Name)
            .WithTags(GetTag(type.Name));
        }
    }

    private static string GetTag(string className)
    {
        if (className.EndsWith("Function"))
            className = className.Replace("Function", "");

        if (className.EndsWith("Endpoint"))
            className = className.Replace("Endpoint", "");

        return className;
    }

    public static async Task<APIGatewayProxyRequest> ToProxyRequest(
        this HttpContext context)
    {
        string body = "";

        if (context.Request.ContentLength > 0)
        {
            using var reader = new StreamReader(context.Request.Body);
            body = await reader.ReadToEndAsync();
        }

        var headers = context.Request.Headers
            .ToDictionary(x => x.Key, x => x.Value.ToString());

        var query = context.Request.Query
            .ToDictionary(x => x.Key, x => x.Value.ToString());

        var routeValues = context.Request.RouteValues
            .ToDictionary(x => x.Key, x => x.Value?.ToString());

        return new APIGatewayProxyRequest
        {
            HttpMethod = context.Request.Method,
            Path = context.Request.Path,
            Body = body,
            Headers = headers,
            QueryStringParameters = query,
            PathParameters = routeValues
        };
    }
}