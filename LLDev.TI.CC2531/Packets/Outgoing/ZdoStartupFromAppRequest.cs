using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
internal sealed class ZdoStartupFromAppRequest(ushort startDelay) : OutgoingPacket(ZToolCmdType.ZdoStartupFromAppReq, 2), IOutgoingPacket
{
    public ushort StartDelay { get; } = startDelay;

    protected override byte[] Data { get; } = [.. BitConverter.GetBytes(startDelay)];
}
