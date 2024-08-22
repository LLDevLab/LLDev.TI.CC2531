using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
public sealed class SysPingRequest() : OutgoingPacket(ZToolCmdType.SysPingReq, 0x00), IOutgoingPacket
{
    protected override byte[] Data => [];
}
