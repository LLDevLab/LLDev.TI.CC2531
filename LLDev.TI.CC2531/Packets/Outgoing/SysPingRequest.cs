using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Outgoing;
internal sealed class SysPingRequest() : OutgoingPacket(ZToolCmdType.SysPingReq, 0), IOutgoingPacket
{
    protected override byte[] Data => [];
}
