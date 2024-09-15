using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Incoming;

internal interface IZbGetDeviceInfoResponse : IIncomingPacket
{
    public DeviceInfoType DeviceInfoType { get; }
    public ulong Value { get; }
}

internal sealed class ZbGetDeviceInfoResponse : IncomingPacket, IZbGetDeviceInfoResponse
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
