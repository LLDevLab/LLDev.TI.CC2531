namespace LLDev.TI.CC2531.RxTx.Models;
public sealed record DiviceAnnounceInfo(ulong IeeeAddr, ushort NwkAddr, ushort SrcAddr, bool IsMainPowered, bool IsReceiverOnWhenIdle, bool IsSecure);
