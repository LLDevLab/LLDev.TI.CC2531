using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;

internal interface IZdoStartupFromAppResponse : IIncomingPacket
{
    public ZToolZdoStartupFromAppStatus Status { get; }
}

internal sealed class ZdoStartupFromAppResponse : IncomingPacket, IZdoStartupFromAppResponse
{
    public ZToolZdoStartupFromAppStatus Status { get; }
    public ZdoStartupFromAppResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x01)
    {
        Status = (ZToolZdoStartupFromAppStatus)Data[0];
    }
}
