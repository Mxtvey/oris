using XProtocol.Serializator;

namespace ProtocolFramework;

public class PlayerConnectedDto
{
    [XField(1)] 
    public string Username; 
}