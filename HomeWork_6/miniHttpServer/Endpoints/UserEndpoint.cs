using MiniHttpServer.Framework.Core.Attributes;
using Microsoft.Data.SqlClient;                    
using MiniHttpServer.Framework.Core.Abstracts;
using HttpServer.Shared;
using System.Text;
using MiniHttpServer.HttpResponce;


namespace MiniHttpServer.Framework.Core.Handlers
{
    internal class UserEndpoint : BaseEndpoint
    {
        [HttpGet("users")]
        public async Task<IResponseResult> GetUsers()
        {
            var settings = SettingsModelSingleton.Instance;
            var connectionString = settings.ConnectionString;

            // создаём ORM
            var orm = new ORMContext(connectionString);

          
            var users = orm.ReadAll<User>("users");

            var sb = new StringBuilder();
            sb.AppendLine("<h2>Список пользователей</h2>");

            foreach (var u in users)
            {
                sb.AppendLine($"{u.Id}\t{u.Name}\t{u.Age}<br>");
            }

            return new PageResult(sb.ToString(), "text/html; charset=utf-8");
        }
    }
}