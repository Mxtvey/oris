namespace XProtocol;

public enum XPacketType : byte
{
    Unknown = 0,
    Handshake = 1,

    PlayerConnected = 10,
    Welcome = 11,
    PlayerJoined = 12,

    PointPlaced = 20
}
