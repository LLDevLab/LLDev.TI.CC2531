namespace LLDev.TI.CC2531.RxTx.Enums;

[Flags]
internal enum ZToolZdoNodeDescRpsServerMask : ushort
{
    PrimaryTrustCenter = 0x01,
    BackupTrustCenter = 0x02,
    PrimaryBindTableCache = 0x04,
    BackupBindTableCache = 0x08,
    PrimaryDiscoveryCache = 0x10,
    BackupDiscoveryCache = 0x20
}
