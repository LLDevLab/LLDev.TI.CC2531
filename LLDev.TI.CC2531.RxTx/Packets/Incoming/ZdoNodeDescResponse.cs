using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;
internal sealed class ZdoNodeDescResponse : IncomingPacket, IIncomingPacket
{
    public ZToolPacketStatus Status { get; }
    public ZdoNodeDescResponse(IPacketHeader header, byte[] data) :
        base(header, data, 0x01)
    {
        Status = (ZToolPacketStatus)Data[0];
    }
}
