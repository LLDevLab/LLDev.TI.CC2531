using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Incoming;

internal interface IZdoExtRouteDiscoveryResponse : IIncomingPacket
{
    public ZToolPacketStatus Status { get; }
}

internal sealed class ZdoExtRouteDiscoveryResponse : IncomingPacket, IZdoExtRouteDiscoveryResponse
{
    public ZToolPacketStatus Status { get; }

    public ZdoExtRouteDiscoveryResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x01)
    {
        Status = (ZToolPacketStatus)Data[0];
    }
}
