
using MiniHttpServer.Shared;
using System.Net;
using System.Text;
using MiniHttpServer.Framework.Core.Abstracts;

namespace MiniHttpServer.Framework.Core.Handlers
{
    internal class StaticFilesHandler : Handler
    {
        public override async void HandleRequest(HttpListenerContext context)
        {

            var request = context.Request;  
            var isGetMethod = request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase);
            var isStaticFile = request.Url.AbsolutePath.Split('/').Any(x=> x.Contains("."));

            if (isGetMethod && isStaticFile)
            {
                var response = context.Response;

                byte[]? buffer = null;

                string path = request.Url.AbsolutePath;
         
                buffer = GetResponseBytes.Invoke(path);

                response.ContentType =ContentType.GetContentType(path);

                if (buffer == null)
                {
                    response.StatusCode = 404;
                    string errorText = "<html><body>404 - Not Found</html></body>";
                    buffer = Encoding.UTF8.GetBytes(errorText);
                }

                response.ContentLength64 = buffer.Length;

                using Stream output = response.OutputStream;
                await output.WriteAsync(buffer, 0, buffer.Length);
                await output.FlushAsync();

                if (response.StatusCode == 200)
                    Console.WriteLine($"Запрос обработан: {request.Url.AbsolutePath} - Status: {response.StatusCode}");
                else
                    Console.WriteLine($"Ошибка запроса: {request.Url.AbsolutePath} - Status: {response.StatusCode}  {path}");

            }
            // передача запроса дальше по цепи при наличии в ней обработчиков
            else if (Successor != null)
            {
                Successor.HandleRequest(context);
            }
        }
    }
}
