using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Incoming;

internal interface IZdoNwkDiscoveryResponse : IIncomingPacket
{
    public ZToolPacketStatus Status { get; }
}

internal sealed class ZdoNwkDiscoveryResponse : IncomingPacket, IZdoNwkDiscoveryResponse
{
    public ZToolPacketStatus Status { get; }
    public ZdoNwkDiscoveryResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x01)
    {
        Status = (ZToolPacketStatus)Data[0];
    }
}
