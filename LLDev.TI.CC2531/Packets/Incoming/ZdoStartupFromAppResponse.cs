using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Incoming;

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
