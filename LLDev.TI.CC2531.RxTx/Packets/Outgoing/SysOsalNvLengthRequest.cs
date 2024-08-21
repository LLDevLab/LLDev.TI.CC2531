using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
public sealed class SysOsalNvLengthRequest(ushort id) : 
    OutgoingPacket(ZToolCmdType.SysOsalNvLengthReq, 0x02), IOutgoingPacket
{
    public ushort Id { get; } = id;

    protected override byte[] Data => BitConverter.GetBytes(Id);
}
