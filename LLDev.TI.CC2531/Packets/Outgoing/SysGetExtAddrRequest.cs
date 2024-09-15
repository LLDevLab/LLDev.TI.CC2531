using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Outgoing;
internal sealed class SysGetExtAddrRequest() : OutgoingPacket(ZToolCmdType.SysGetExtAddrReq, 0), IOutgoingPacket
{
    protected override byte[] Data { get; } = [];
}
