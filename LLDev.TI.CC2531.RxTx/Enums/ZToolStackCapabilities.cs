namespace LLDev.TI.CC2531.RxTx.Enums;

[Flags]
public enum ZToolStackCapabilities : ushort
{
    MtCapSys = 0x0001,
    MtCapMac = 0x0002,
    MtCapNwk = 0x0004,
    MtCapAf = 0x0008,
    MtCapZdo = 0x0010,
    MtCapSapi = 0x0020,
    MtCapUtil = 0x0040,
    MtCapDebug = 0x0080,
    MtCapApp = 0x0100,
    MtCapZoad = 0x1000
}
