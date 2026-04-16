using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json.Serialization;
using System.ComponentModel;
using Newtonsoft.Json;

namespace FastLambdaEndpoint;

public static class APIGatewayProxyRequestExtensions
{
    /// <summary>
    /// Busca no header por "x-api-key"
    /// </summary>
    /// <param name="proxyRequest"></param>
    /// <returns></returns>
    public static string XApiKey(this APIGatewayProxyRequest proxyRequest)
    {
        proxyRequest.Headers.TryGetValue("x-api-key", out string result);
        return result;
    }

    /// <summary>
    /// Busca no header por "x-client-id"
    /// </summary>
    /// <param name="proxyRequest"></param>
    /// <returns></returns>
    public static string XClientId(this APIGatewayProxyRequest proxyRequest)
    {
        proxyRequest.Headers.TryGetValue("x-client-id", out string result);
        return result;
    }

    public static T Header<T>(this APIGatewayProxyRequest proxyRequest, string paramName)
    {
        return proxyRequest.Get<T>(proxyRequest?.Headers, paramName);
    }

    public static T Route<T>(this APIGatewayProxyRequest proxyRequest, string paramName)
    {
        return proxyRequest.Get<T>(proxyRequest?.PathParameters, paramName);
    }

    public static T Query<T>(this APIGatewayProxyRequest proxyRequest, string paramName)
    {
        return proxyRequest.Get<T>(proxyRequest?.QueryStringParameters, paramName);
    }

    private static T Get<T>(this APIGatewayProxyRequest proxyRequest, IDictionary<string, string>? dic, string paramName)
    {
        if (dic is null)
            return default;

        if (!dic.TryGetValue(paramName, out string output) || string.IsNullOrEmpty(output))
            return default;

        try
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null && converter.CanConvertFrom(typeof(string)))
                return (T)converter.ConvertFromString(output);
            return (T)Convert.ChangeType(output, typeof(T));
        }
        catch (Exception)
        {
            return default;
        }
    }

    public static T ToModel<T>(this APIGatewayProxyRequest proxyRequest)
    {
        var camelSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        return JsonConvert.DeserializeObject<T>(proxyRequest.Body, settings: camelSettings);
    }
}