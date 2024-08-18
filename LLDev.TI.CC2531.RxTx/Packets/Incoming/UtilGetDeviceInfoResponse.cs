using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;
public sealed class UtilGetDeviceInfoResponse : IncomingPacket, IIncomingPacket
{
    public ZToolPacketStatus Status { get; }
    public ulong IeeeAddr { get; }
    public ushort NwkAddr { get; }
    public UtilGetDeviceInfoDeviceState State { get; }
    public byte NumAssocDevices { get; }
    public ushort[] AssocDevices { get; }
    public bool IsCoordinatorCapable => _deviceType.HasFlag(ZigBeeDeviceTypeFlags.Coordinator);
    public bool IsRouterCapable => _deviceType.HasFlag(ZigBeeDeviceTypeFlags.Router);
    public bool IsEndDeviceCapable => _deviceType.HasFlag(ZigBeeDeviceTypeFlags.EndDevice);

    private readonly ZigBeeDeviceTypeFlags _deviceType;

    public UtilGetDeviceInfoResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x0e)
    {
        Status = (ZToolPacketStatus)Data[0];
        IeeeAddr = GetBigEndianULong(Data[1..9]);
        NwkAddr = GetUShort(Data[9], Data[10]);
        _deviceType = (ZigBeeDeviceTypeFlags)Data[11];
        State = (UtilGetDeviceInfoDeviceState)Data[12];
        NumAssocDevices = Data[13];
        AssocDevices = new ushort[NumAssocDevices];

        var deviceStartIdx = 14;
        for (var i = 0; i < NumAssocDevices; i++)
        {
            AssocDevices[i] = GetUShort(Data[deviceStartIdx + 1], Data[deviceStartIdx]);
            deviceStartIdx += 2;
        }
    }
}
