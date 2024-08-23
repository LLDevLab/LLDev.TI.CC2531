using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
public sealed class ZdoMsgCbRegisterRequest(ushort cluster) : OutgoingPacket(ZToolCmdType.ZdoMsgCbRegisterReq, 2), IOutgoingPacket
{
    protected override byte[] Data { get; } = [.. BitConverter.GetBytes(cluster)];
}
