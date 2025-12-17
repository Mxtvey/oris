using System.Net;

namespace MiniHttpServer.HttpResponce;

public class PageResult : IResponseResult
{   
    private readonly object _data;
    private readonly string _pathTemplate;

    public PageResult(string pathTemplate, object data)
    {
        
    }
    public void Execute(HttpListenerContext context)
    {
        throw new NotImplementedException();
    }
}