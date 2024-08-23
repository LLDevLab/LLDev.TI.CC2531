using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
public sealed class UtilGetDeviceInfoRequest() : OutgoingPacket(ZToolCmdType.UtilGetDeviceInfoReq, 0), IOutgoingPacket
{
    protected override byte[] Data => [];
}
