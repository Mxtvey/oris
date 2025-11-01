using System.Net;

namespace MiniHttpServer.HttpResponce;

public abstract class BaseEndpoint
{
    protected HttpListenerContext Context {get; private set;}

    public void SetContext(HttpListenerContext context)
    {
        Context = context;
    }
    
    protected IResponseResult Page(string pathTamplate , object data ) => new PageResult(pathTamplate, data);
}