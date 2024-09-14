using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
internal sealed class UtilLedControlRequest : OutgoingPacket, IOutgoingPacket
{
    public byte LedId { get; }
    public bool LedOn { get; }
    protected override byte[] Data { get; }

    public UtilLedControlRequest(byte ledId, bool ledOn) :
        base(ZToolCmdType.UtilLedControlReq, 2)
    {
        LedId = ledId;
        LedOn = ledOn;
        Data = [LedId, (byte)(LedOn ? 1 : 0)];
    }
}
