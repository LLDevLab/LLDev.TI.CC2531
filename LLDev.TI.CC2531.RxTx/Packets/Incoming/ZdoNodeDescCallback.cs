using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;
public sealed class ZdoNodeDescCallback : IncomingPacket, IIncomingPacket
{
    public ZToolPacketStatus Status { get; }
    public ZdoNodeDescCallback(IPacketHeader header, byte[] data) :
        base(header, data, 0x01)
    {
        Status = (ZToolPacketStatus)Data[0];
    }
}
