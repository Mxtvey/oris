using System;
using System.Net;
using System.IO;
    using System.Text;
using HttpServer.Shared;

namespace miniHttpServer
{
    public class HttpServer
    {

        private SettingsModel settings;
        private HttpListener _listener;
        public HttpServer(SettingsModel config) => settings = config;

        public void Start()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://" + settings.Domain + ":" + settings.Port + "/");
            _listener.Start();
            Console.WriteLine("Server is started");
            Receive();  
            Console.WriteLine("Server is awaiting for request");
        }

        public void Stop()
        {
            _listener.Stop();
            Console.WriteLine("Server is stopped");
        }

        private void Receive()
        {
            _listener.BeginGetContext(new AsyncCallback(ListenerCallbackAsync), _listener);
        }

        private async void ListenerCallbackAsync(IAsyncResult result)
        {
            if (_listener.IsListening)
            {  
                var context = _listener.EndGetContext(result);
                var responce = context.Response;
                try
                {
                    string responseText = File.ReadAllText(settings.StaticDirectoryPath + "index.html");
                    byte[] buffer = Encoding.UTF8.GetBytes(responseText);
                    responce.ContentLength64 = buffer.Length;
                    using Stream output = responce.OutputStream;
                    await output.WriteAsync(buffer);
                    await output.FlushAsync();
                    Console.WriteLine("Запрос обработан");
                }
                catch (DirectoryNotFoundException)
                {
                    Console.WriteLine("static folder is not found");
                    _listener.Stop();
                    Console.WriteLine("Server is stopped");
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("index.html is not found in static folder");
                    _listener.Stop();
                    Console.WriteLine("Server is stopped");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("There is an exception" + ex.Message);
                    _listener.Stop();
                    Console.WriteLine("Server is stopped");
                }
                Receive();
            }
        }
    }
}
