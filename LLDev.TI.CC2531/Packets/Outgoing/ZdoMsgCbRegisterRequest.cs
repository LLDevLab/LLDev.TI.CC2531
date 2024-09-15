using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Outgoing;
internal sealed class ZdoMsgCbRegisterRequest(ushort cluster) : OutgoingPacket(ZToolCmdType.ZdoMsgCbRegisterReq, 2), IOutgoingPacket
{
    protected override byte[] Data { get; } = [.. BitConverter.GetBytes(cluster)];
}
