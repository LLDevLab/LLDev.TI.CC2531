using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Outgoing;
internal sealed class ZdoExtRouteDiscoveryRequest(ushort destAddr, byte options, byte radius) : OutgoingPacket(ZToolCmdType.ZdoExtRouteDicsReq, 5), IOutgoingPacket
{
    public ushort DestAddr { get; } = destAddr;
    public byte Options { get; } = options;
    public byte Radius { get; } = radius;

    protected override byte[] Data { get; } = [.. BitConverter.GetBytes(destAddr), options, radius];
}
