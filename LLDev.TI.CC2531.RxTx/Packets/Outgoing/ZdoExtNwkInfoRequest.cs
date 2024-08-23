using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
public sealed class ZdoExtNwkInfoRequest() : OutgoingPacket(ZToolCmdType.ZdoExtNwkInfoReq, 0), IOutgoingPacket
{
    protected override byte[] Data => [];
}
