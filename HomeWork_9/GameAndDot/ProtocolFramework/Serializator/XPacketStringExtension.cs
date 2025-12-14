namespace XProtocol.Serializator;
using System.Text;
using XProtocol;
using System.Text;

public static class XPacketStringExtensions
{
    public static void SetString(this XPacket p, byte fieldId, string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value ?? "");
        p.SetValueRaw(fieldId, bytes);
    }

    public static string GetString(this XPacket p, byte fieldId)
    {
        var bytes = p.GetValueRaw(fieldId);
        return Encoding.UTF8.GetString(bytes);
    }
}
