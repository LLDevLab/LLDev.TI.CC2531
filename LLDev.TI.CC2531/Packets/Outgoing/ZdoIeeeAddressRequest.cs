using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
internal sealed class ZdoIeeeAddressRequest(ushort nwkAddr, IeeeAddressRequestType reqType, byte startIndex) :
    OutgoingPacket(ZToolCmdType.ZdoIeeeAddrReq, 4), IOutgoingPacket
{
    public ushort NwkAddr { get; } = nwkAddr;
    public IeeeAddressRequestType ReqType { get; } = reqType;
    public byte StartIndex { get; } = startIndex;

    protected override byte[] Data { get; } = [.. BitConverter.GetBytes(nwkAddr), (byte)reqType, startIndex];
}
