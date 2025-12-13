using System.Net;
using System.Text;

HttpListener server = new HttpListener();
server.Prefixes.Add("http://127.0.0.1:8888/");
server.Start();

Console.WriteLine("Server started");

while (true)
{
    var context = await server.GetContextAsync();
    HandleRequest(context);
}

void HandleRequest(HttpListenerContext context)
{
    var request = context.Request;
    var response = context.Response;

    if (request.HttpMethod == "GET" &&  request.Url.AbsolutePath == "/health")
    {
        response.StatusCode = 200;
        response.ContentType = "text/plain";

        byte[] buffer = Encoding.UTF8.GetBytes("OK");
        response.OutputStream.Write(buffer, 0, buffer.Length);

        response.Close();
        return;
    }
    if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/config")
    {   
        string full = Path.GetFullPath("Config.json");
        string jsonq = File.ReadAllText(full);
        string json = System.Text.Json.JsonSerializer.Serialize(jsonq);
       
        byte[] buffer = Encoding.UTF8.GetBytes(json);
        response.StatusCode = 200;
        response.ContentType = "application/json";

        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.Close();
        return;
    }

    
    response.Close();
}