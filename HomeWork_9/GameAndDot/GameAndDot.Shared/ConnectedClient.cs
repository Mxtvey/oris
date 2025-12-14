using System.Net.Sockets;
using XProtocol;
using XProtocol.Serializator;

namespace GameAndDot.Shared;

public class ConnectedClient
{
    public Socket Client { get; }

    private readonly Queue<byte[]> _packetSendingQueue = new Queue<byte[]>();

    private readonly XServer _server;

    public int PlayerId { get; }
    public string Username { get; private set; } = "";
    public string ColorHex { get; private set; } = "";

    public ConnectedClient(Socket client, XServer server, int playerId)
    {
        Client = client;
        _server = server;
        PlayerId = playerId;

        Task.Run((Action)ProcessIncomingPackets);
        Task.Run((Action)SendPackets);
    }

    private void ProcessIncomingPackets()
    {
        try
        {
            while (true)
            {
                var buff = new byte[256];
                Client.Receive(buff);

                buff = buff.TakeWhile((b, i) =>
                {
                    if (b != 0xFF) return true;
                    return buff[i + 1] != 0;
                }).Concat(new byte[] { 0xFF, 0 }).ToArray();

                var parsed = XPacket.Parse(buff);
                if (parsed != null)
                    ProcessIncomingPacket(parsed);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("CLIENT LOOP CRASH: " + ex);
            Client.Close();
        }
    }

    private void ProcessIncomingPacket(XPacket packet)
    {
        var type = XPacketTypeManager.GetTypeFromPacket(packet);

        switch (type)
        {
            case XPacketType.Handshake:
                ProcessHandshake(packet);
                break;

            case XPacketType.PlayerConnected:
                ProcessPlayerConnected(packet);
                break;

            case XPacketType.PointPlaced:
                _server.Broadcast(packet);
                break;

            case XPacketType.Unknown:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ProcessPlayerConnected(XPacket packet)
    {
        Username = System.Text.Encoding.UTF8.GetString(packet.GetValueRaw(1));
        ColorHex = _server.GetOrCreateColor(Username);

        Console.WriteLine($"SERVER: PlayerConnected id={PlayerId} user={Username}");

        // 1. Welcome — ТОЛЬКО новому
        var welcome = XPacket.Create(XPacketType.Welcome);
        welcome.SetValue(1, PlayerId);
        QueuePacketSend(welcome.ToPacket());

        // 2. PlayerJoined ПРО СЕБЯ — новому
        var selfJoined = _server.MakePlayerJoinedPacket(this);
        QueuePacketSend(selfJoined.ToPacket());

        // 3. PlayerJoined ПРО ОСТАЛЬНЫХ — новому
        foreach (var other in _server.ClientsSnapshot())
        {
            if (other == this) continue;
            QueuePacketSend(_server.MakePlayerJoinedPacket(other).ToPacket());
        }

        // 4. PlayerJoined ПРО НЕГО — ВСЕМ ОСТАЛЬНЫМ
        _server.BroadcastExcept(this, selfJoined);

        Console.WriteLine($"SERVER: Broadcast PlayerJoined for id={PlayerId}");
    }


    private void ProcessHandshake(XPacket packet)
    {
        Console.WriteLine("Recieved handshake packet.");

        var handshake = XPacketConverter.Deserialize<XPacketHandshake>(packet);
        handshake.MagicHandshakeNumber -= 15;
        
        Console.WriteLine("Answering..");

        QueuePacketSend(XPacketConverter.Serialize(XPacketType.Handshake, handshake).ToPacket());
    }

    public void QueuePacketSend(byte[] packet)
    {
        if (packet.Length > 256)
        {
            throw new Exception("Max packet size is 256 bytes.");
        }

        _packetSendingQueue.Enqueue(packet);
    }

    private void SendPackets()
    {
        while (true)
        {
            if (_packetSendingQueue.Count == 0)
            {
                Thread.Sleep(100);
                continue;
            }

            var packet = _packetSendingQueue.Dequeue();
            Client.Send(packet);

            Thread.Sleep(100);
        }
    }
}