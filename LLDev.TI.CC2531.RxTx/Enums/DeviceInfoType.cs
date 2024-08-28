namespace LLDev.TI.CC2531.RxTx.Enums;
internal enum DeviceInfoType : byte
{
    State = 0x00,
    IeeeAddr = 0x01,
    ShortAddr = 0x02,
    ParentShortAddr = 0x03,
    ParentIeeeAddr = 0x04,
    Channel = 0x05,
    PanId = 0x06,
    ExtPanId = 0x07
}
