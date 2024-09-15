using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Outgoing;
internal sealed class SysVersionRequest() : OutgoingPacket(ZToolCmdType.SysVersionReq, 0), IOutgoingPacket
{
    protected override byte[] Data => [];
}
