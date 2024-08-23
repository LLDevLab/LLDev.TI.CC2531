namespace LLDev.TI.CC2531.RxTx.Enums;
/// <summary>
/// Bit mask for channels to scan.
/// </summary>
[Flags]
public enum ZToolDiscoveryScanChannelTypes : uint
{
    None = 0x00,
    Channel_11 = 0x00000800,
    Channel_12 = 0x00001000,
    Channel_13 = 0x00002000,
    Channel_14 = 0x00004000,
    Channel_15 = 0x00008000,
    Channel_16 = 0x00010000,
    Channel_17 = 0x00020000,
    Channel_18 = 0x00040000,
    Channel_19 = 0x00080000,
    Channel_20 = 0x00100000,
    Channel_21 = 0x00200000,
    Channel_22 = 0x00400000,
    Channel_23 = 0x00800000,
    Channel_24 = 0x01000000,
    Channel_25 = 0x02000000,
    Channel_26 = 0x04000000,
    All = 0x07FFF800
}
