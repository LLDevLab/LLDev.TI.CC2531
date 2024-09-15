using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Outgoing;
internal sealed class SysResetRequest : OutgoingPacket, IOutgoingPacket
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
