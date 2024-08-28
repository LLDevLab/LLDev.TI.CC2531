namespace LLDev.TI.CC2531.RxTx.Enums;

[Flags]
internal enum ZigBeeDeviceTypeFlags : byte
{
    Coordinator = 0x01,
    Router = 0x02,
    EndDevice = 0x04
}
