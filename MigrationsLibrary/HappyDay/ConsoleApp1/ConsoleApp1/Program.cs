using Library;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using ConsoleApp1;

partial class Program
{
    static async Task Main()
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:5000/");
        listener.Start();
        Console.WriteLine("HTTP API: http://localhost:5000");

        var db = new Conection("Host=localhost;Username=postgres;Password=1234;Database=Happy");
        var service = new MigrationService(db, typeof(User).Assembly);
        
        while (true)
        {
            var ctx = await listener.GetContextAsync();
            _ = Process(ctx, service);
        }
    }

    static async Task Process(HttpListenerContext ctx, MigrationService svc)
    {
        string path = ctx.Request.Url.AbsolutePath;
        Console.WriteLine("PATH = " + path);

        if (path == "/favicon.ico")
        {
            ctx.Response.StatusCode = 404;
            ctx.Response.Close();
            return;
        }

        object result;

        if (path.StartsWith("/migrate/create"))
        {
            result = svc.Create();
        }
        else if (path.StartsWith("/migrate/apply"))
        {
            result = svc.Apply();
        }
        else
        {
            result = new { error = "not_found" };
        }

        string json = System.Text.Json.JsonSerializer.Serialize(result);
        Console.WriteLine("JSON = " + json);

        byte[] bytes = Encoding.UTF8.GetBytes(json);
        ctx.Response.ContentType = "application/json";
        ctx.Response.ContentEncoding = Encoding.UTF8;
        ctx.Response.ContentLength64 = bytes.Length;

        await ctx.Response.OutputStream.WriteAsync(bytes);
        ctx.Response.Close();
    }

}