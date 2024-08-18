using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;
public sealed class ZdoStartupFromAppResponse : IncomingPacket, IIncomingPacket
{
    public ZToolZdoStartupFromAppStatus Status { get; }
    public ZdoStartupFromAppResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x01)
    {
        Status = (ZToolZdoStartupFromAppStatus)Data[0];
    }
}
