using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;

public sealed class SysPingResponse : IncomingPacket, IIncomingPacket
{
    public bool IsSysCapable => _capabilities.HasFlag(ZToolStackCapabilities.MtCapSys);
    public bool IsMacCapable => _capabilities.HasFlag(ZToolStackCapabilities.MtCapMac);
    public bool IsNwkCapable => _capabilities.HasFlag(ZToolStackCapabilities.MtCapNwk);
    public bool IsAfCapable => _capabilities.HasFlag(ZToolStackCapabilities.MtCapAf);
    public bool IsZdoCapable => _capabilities.HasFlag(ZToolStackCapabilities.MtCapZdo);
    public bool IsSapiCapable => _capabilities.HasFlag(ZToolStackCapabilities.MtCapSapi);
    public bool IsUtilCapable => _capabilities.HasFlag(ZToolStackCapabilities.MtCapUtil);
    public bool IsDebugCapable => _capabilities.HasFlag(ZToolStackCapabilities.MtCapDebug);
    public bool IsAppCapable => _capabilities.HasFlag(ZToolStackCapabilities.MtCapApp);
    public bool IsZoadCapable => _capabilities.HasFlag(ZToolStackCapabilities.MtCapZoad);

    private readonly ZToolStackCapabilities _capabilities;

    public SysPingResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x02)
    {
        _capabilities = (ZToolStackCapabilities)GetUShort(Data[1], Data[0]);
    }
}
