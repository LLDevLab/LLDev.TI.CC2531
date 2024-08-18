using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;
public sealed class ZdoSimpleDescResponse : IncomingPacket, IIncomingPacket
{
    public ZToolPacketStatus Status { get; }

    public ZdoSimpleDescResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x01)
    {
        Status = (ZToolPacketStatus)Data[0];
    }
}
