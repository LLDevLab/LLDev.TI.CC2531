using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;

internal sealed class ZbGetDeviceInfoResponse : IncomingPacket, IIncomingPacket
{
    public DeviceInfoType DeviceInfoType { get; }
    public ulong Value { get; }

    public ZbGetDeviceInfoResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x09)
    {
        DeviceInfoType = (DeviceInfoType)Data[0];
        Value = GetValue();
    }

    private ulong GetValue()
    {
        var data = Data[1..9];
        return DeviceInfoType switch
        {
            DeviceInfoType.Channel => data[0],
            DeviceInfoType.PanId => GetLittleEndianULong(data[..2]),
            _ => GetBigEndianULong(data),
        };
    }
}
