using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Incoming;

internal interface IZbWriteConfigResponse : IIncomingPacket
{
    public ZToolPacketStatus Status { get; }
}

internal sealed class ZbWriteConfigResponse : IncomingPacket, IZbWriteConfigResponse
{
    public ZToolPacketStatus Status { get; }
    public ZbWriteConfigResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x01)
    {
        Status = (ZToolPacketStatus)Data[0];
    }
}
