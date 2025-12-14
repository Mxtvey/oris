using GameAndDot.Shared;
using XProtocol;

namespace GameAndDot.Server
{
    internal class Program
    {
        private static void Main()
        {
            Console.Title = "XServer";
            Console.ForegroundColor = ConsoleColor.White;
            XPacketTypeManager.RegisterType(XPacketType.Handshake, 1, 0);
            XPacketTypeManager.RegisterType(XPacketType.PlayerConnected, 1, 1);
            XPacketTypeManager.RegisterType(XPacketType.Welcome, 1, 2);
            XPacketTypeManager.RegisterType(XPacketType.PlayerJoined, 1, 3);
            XPacketTypeManager.RegisterType(XPacketType.PointPlaced, 1, 4);


            var server = new XServer();
            server.Start();
            server.AcceptClients();
        }
    }
}
