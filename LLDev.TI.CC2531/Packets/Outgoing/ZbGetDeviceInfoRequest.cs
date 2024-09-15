using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Outgoing;
internal sealed class ZbGetDeviceInfoRequest : OutgoingPacket, IOutgoingPacket
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
