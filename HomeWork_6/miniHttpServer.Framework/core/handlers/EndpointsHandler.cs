
using System.Net;
using System.Reflection;

using MiniHttpServer.Core.Attributes;
using MiniHttpServer.Framework.Core.Abstracts;
using MiniHttpServer.Framework.Core.Attributes;
using MiniHttpServer.HttpResponce;


internal class EndpointsHandler : Handler
{
    public override void HandleRequest(HttpListenerContext context)
    {
        if (true)
        {
            var request = context.Request;
            var endpointName = request.Url?.AbsolutePath.Split('/')[1];

            var assembly = Assembly.GetExecutingAssembly();
            var endpoint = assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<EndpointAttribute>() != null)
                .FirstOrDefault(end => IsMatch(end.Name, endpointName));

            bool isBaseEndpoint = endpoint.Assembly.GetTypes()
                    .Any(t => typeof(BaseEndpoint)
                    .IsAssignableFrom(t) && !t.IsAbstract);
            var instanceEndpoint = Activator.CreateInstance(endpoint);

            if (isBaseEndpoint)
            {
                (instanceEndpoint as BaseEndpoint).SetContext(context); 
            }

            if (endpoint == null) return; // TODO:

            var method = endpoint.GetMethods().Where(t => t.GetCustomAttributes(true)
                    .Any(attr => attr.GetType().Name.Equals($"Http{context.Request.HttpMethod}",
                        StringComparison.OrdinalIgnoreCase)))
                .FirstOrDefault();

            if (method == null) return; // TODO:

            var res = method.Invoke(Activator.CreateInstance(endpoint), null);


            // var httpAttrName = $"Http{request.HttpMethod}";
            // var methods = endpointType.GetMethods()
            //     .Where(m => m.GetCustomAttributes(true)
            //         .Any(attr => string.Equals(attr.GetType().Name, httpAttrName, StringComparison.OrdinalIgnoreCase)))
            //     .ToList();
            //
            // if (methods.Count == 0)
            // {
            //     Successor?.HandleRequest(context);
            //     return;
            // }
            //
            // var chosen = methods.FirstOrDefault(m =>
            // {
            //     var attr = m.GetCustomAttributes(true)
            //         .First(a => string.Equals(a.GetType().Name, httpAttrName, StringComparison.OrdinalIgnoreCase));
            //
            //     var routeProp = attr.GetType().GetProperty("Route");
            //     var route = (routeProp?.GetValue(attr) as string)?.Trim('/') ?? string.Empty;
            //
            //     return string.IsNullOrEmpty(route)
            //         ? string.IsNullOrEmpty(routeTail)
            //         : string.Equals(route, routeTail, StringComparison.OrdinalIgnoreCase);
            // }) ?? methods.First();

            // var instance = Activator.CreateInstance(endpointType);

        //     object? result = null;
        //     try
        //     {
        //         var ps = chosen.GetParameters();
        //         if (ps.Length == 1 && ps[0].ParameterType == typeof(HttpListenerContext))
        //             result = chosen.Invoke(instance, new object[] { context });
        //         else
        //             result = chosen.Invoke(instance, null);
        //
        //         if (result is Task t)
        //             t.GetAwaiter().GetResult();
        //     }
        //     catch
        //     {
        //         context.Response.StatusCode = 500;
        //     }
        }

        bool IsMatch(string typeName, string slug)
        {
            return typeName.Equals(slug, StringComparison.OrdinalIgnoreCase) ||
                   typeName.Equals($"{slug}Endpoint", StringComparison.OrdinalIgnoreCase);
        }
    }
}
