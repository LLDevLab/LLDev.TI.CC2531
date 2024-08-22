using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
public sealed class SysResetRequest : OutgoingPacket, IOutgoingPacket
{
    public ZToolSysResetType ResetType { get; }
    protected override byte[] Data { get; }

    public SysResetRequest(ZToolSysResetType resetType) :
        base(ZToolCmdType.SysResetReq, 1)
    {
        ResetType = resetType;
        Data = [(byte)ResetType];
    }
}
