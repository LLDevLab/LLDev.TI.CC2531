using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
public sealed class SysGetExtAddrRequest : OutgoingPacket, IOutgoingPacket
{
    protected override byte[] Data { get; }

    public SysGetExtAddrRequest() :
        base(ZToolCmdType.SysGetExtAddrReq, 0x00)
    {
        Data = [];
    }
}
