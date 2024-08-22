using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
public sealed class ZbGetDeviceInfoRequest : OutgoingPacket, IOutgoingPacket
{
    public DeviceInfoType InfoType { get; }
    protected override byte[] Data { get; }

    public ZbGetDeviceInfoRequest(DeviceInfoType infoType) :
        base(ZToolCmdType.ZbGetDeviceInfoReq, 1)
    {
        InfoType = infoType;
        Data = [(byte)InfoType];
    }
}
