using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
public sealed class SysVersionRequest() : OutgoingPacket(ZToolCmdType.SysVersionReq, 0x00), IOutgoingPacket
{
    protected override byte[] Data => [];
}
