using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;
internal sealed class ZbWriteConfigResponse : IncomingPacket, IIncomingPacket
{
    public ZToolPacketStatus Status { get; }
    public ZbWriteConfigResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x01)
    {
        Status = (ZToolPacketStatus)Data[0];
    }
}
