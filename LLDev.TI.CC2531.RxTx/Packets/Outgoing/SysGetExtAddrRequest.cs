using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
public sealed class SysGetExtAddrRequest() : OutgoingPacket(ZToolCmdType.SysGetExtAddrReq, 0), IOutgoingPacket
{
    protected override byte[] Data { get; } = [];
}
