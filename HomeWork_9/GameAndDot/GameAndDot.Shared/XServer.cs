using System.Net;
using System.Net.Sockets;
using XProtocol;

namespace GameAndDot.Shared;

public class XServer
{
    private readonly Socket _socket;
    private readonly List<ConnectedClient> _clients;
    private int _nextPlayerId = 1;
    public Dictionary<string, string> PlayerColors = new();

    private bool _listening;
    private bool _stopListening;

    public XServer()
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _clients = new List<ConnectedClient>();
    }
    
    public string GenerateUniqueColor()
    {
        var rnd = new Random();

        while (true)
        {
            string color = $"#{rnd.Next(0x1000000):X6}"; 

            if (!PlayerColors.ContainsValue(color))
                return color;
        }
    }

    public void Start()
    {
        if (_listening)
        {
            throw new Exception("Server is already listening incoming requests.");
        }

        _socket.Bind(new IPEndPoint(IPAddress.Any, 4910));
        _socket.Listen(10);

        _listening = true;
    }

    public void Stop()
    {
        if (!_listening)
        {
            throw new Exception("Server is already not listening incoming requests.");
        }

        _stopListening = true;
        _socket.Shutdown(SocketShutdown.Both);
        _listening = false;
    }

    public void AcceptClients()
    {
        while (true)
        {
            if (_stopListening)
            {
                return;
            }

            Socket client;

            try
            {
                client = _socket.Accept();
            } catch { return; }

            Console.WriteLine($"[!] Accepted client from {(IPEndPoint) client.RemoteEndPoint}");

            var id = _nextPlayerId++;
            var c = new ConnectedClient(client, this, id);
            lock (_clients) _clients.Add(c);

        }
    }
    
    public IEnumerable<ConnectedClient> ClientsSnapshot()
    {
        lock (_clients) return _clients.ToList();
    }

    public string GetOrCreateColor(string username)
    {
        lock (PlayerColors)
        {
            if (!PlayerColors.ContainsKey(username))
                PlayerColors[username] = GenerateUniqueColor();
            return PlayerColors[username];
        }
    }

    public XPacket MakePlayerJoinedPacket(ConnectedClient c)
    {
        var p = XPacket.Create(XPacketType.PlayerJoined);
        p.SetValue(1, c.PlayerId);
        p.SetValueRaw(2, System.Text.Encoding.UTF8.GetBytes(c.Username));
        p.SetValueRaw(3, System.Text.Encoding.UTF8.GetBytes(c.ColorHex));
        return p;
    }

    public void Broadcast(XPacket packet)
    {
        var bytes = packet.ToPacket();
        foreach (var c in ClientsSnapshot())    
            c.QueuePacketSend(bytes);
         Console.WriteLine($"SERVER: Broadcast {XPacketTypeManager.GetTypeFromPacket(packet)}");

    }
    public void BroadcastExcept(ConnectedClient except, XPacket packet)
    {
        var bytes = packet.ToPacket();
        foreach (var c in ClientsSnapshot())
        {
            if (c == except) continue;
            c.QueuePacketSend(bytes);
        }
    }


}