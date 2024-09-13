using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;

internal interface IZdoEndDeviceAnnceIndCallback : IIncomingPacket
{
    public ushort SrcAddr { get; }
    public ushort NwkAddr { get; }
    public ulong IeeeAddr { get; }
    public bool IsAlternativePanCoordinator { get; }
    public bool IsZigBeeRouter { get; }
    public bool IsMainPowered { get; }
    public bool IsReceiverOnWhenIdle { get; }
    public bool IsSecure { get; }
}

internal sealed class ZdoEndDeviceAnnceIndCallback : IncomingPacket, IZdoEndDeviceAnnceIndCallback
{
    public ushort SrcAddr { get; }
    public ushort NwkAddr { get; }
    public ulong IeeeAddr { get; }
    public bool IsAlternativePanCoordinator => _capabilities.HasFlag(ZToolEndDevCaps.AltPanCoord);
    public bool IsZigBeeRouter => _capabilities.HasFlag(ZToolEndDevCaps.DeviceType);
    public bool IsMainPowered => _capabilities.HasFlag(ZToolEndDevCaps.PowerSource);
    public bool IsReceiverOnWhenIdle => _capabilities.HasFlag(ZToolEndDevCaps.ReceiverOnWhenIdle);
    public bool IsSecure => _capabilities.HasFlag(ZToolEndDevCaps.Security);

    private readonly ZToolEndDevCaps _capabilities;
    public ZdoEndDeviceAnnceIndCallback(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x0d)
    {
        SrcAddr = GetUShort(Data[1], Data[0]);
        NwkAddr = GetUShort(Data[3], Data[2]);
        IeeeAddr = GetLittleEndianULong(Data[4..12]);
        _capabilities = (ZToolEndDevCaps)Data[12];
    }
}
