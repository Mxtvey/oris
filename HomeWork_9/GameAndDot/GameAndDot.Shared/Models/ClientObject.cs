//using GameAndDot.Shared.Enums;
//using System.Net.Sockets;
//using System.Text.Json;

//namespace GameAndDot.Shared.Models
//{
//    public class ClientObject
//    {
//        protected internal string Id { get; } = Guid.NewGuid().ToString();
//        protected internal StreamWriter Writer { get; }
//        protected internal StreamReader Reader { get; }
//        public string Username { get; set; } = String.Empty;

//        TcpClient client;

//        ServerObject server; // объект сервера

//        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
//        {
//            client = tcpClient;
//            server = serverObject;
//            // получаем NetworkStream для взаимодействия с сервером
//            var stream = client.GetStream();
//            // создаем StreamReader для чтения данных
//            Reader = new StreamReader(stream);
//            // создаем StreamWriter для отправки данных
//            Writer = new StreamWriter(stream);
//        }

//        public async Task ProcessAsync()
//        {
//            try
//            {
//                while (true)
//                {
//                    var json = await Reader.ReadLineAsync();
//                    if (json == null) break;

//                    var messageRequest = JsonSerializer.Deserialize<JsonElement>(json);
//                    var type = (EventType)messageRequest.GetProperty("Type").GetInt32();

//                    switch (type)
//                    {
//                        case EventType.PlayerConnected:
//                            Username = messageRequest.GetProperty("Username").GetString();

//                            Console.WriteLine($"{Username} вошел в чат");
//                            if (!server.PlayerColors.ContainsKey(Username))
//                            {
//                                server.PlayerColors[Username] = server.GenerateUniqueColor();
//                            }

//                            string myColor = server.PlayerColors[Username];

//                            var messageResponse = new EventMessage()
//                            {
//                                Type = EventType.PlayerConnected,
//                                Username = Username,
//                                Id = Id,
//                                Players = server.Clients.Select(c => c.Username).ToList(),
//                                Color = myColor

//                            };

//                            string jsonResponse = JsonSerializer.Serialize(messageResponse);
//                            await server.BroadcastMessageAllAsync(jsonResponse);
//                            break;

//                        case EventType.PointPlaced:
//                            await server.BroadcastMessageAsync(json, Id);
//                            break;
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e.Message);
//            }
//            finally
//            {
//                server.RemoveConnection(Id);
//            }
//        }

//        // закрытие подключения
//        protected internal void Close()
//        {
//            Writer.Close();
//            Reader.Close();
//            client.Close();
//        }
//    }
//}
