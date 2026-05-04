[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class LambdaRouteAttribute : Attribute
{
    public string Method { get; }
    public string Route { get; }

    public LambdaRouteAttribute(string method, string route)
    {
        Method = method.ToUpper();
        Route = route;
    }
}