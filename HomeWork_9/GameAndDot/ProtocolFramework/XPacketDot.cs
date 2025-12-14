namespace ProtocolFramework;

using XProtocol.Serializator;

public class PointPlacedDto
{
    [XField(1)] 
    public int X;
    [XField(2)] 
    public int Y;
    [XField(3)]
    public byte Color;  

}
