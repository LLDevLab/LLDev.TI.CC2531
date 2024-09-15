namespace LLDev.TI.CC2531.Models;
public sealed record DeviceAnnounceInfo(ulong IeeeAddr, ushort NwkAddr, ushort SrcAddr, bool IsMainPowered, bool IsReceiverOnWhenIdle, bool IsSecure);
