using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
internal sealed class ZdoSimpleDescRequest(ushort destAddr, ushort nwkAddrOfInterest, byte endpoint) :
    OutgoingPacket(ZToolCmdType.ZdoSimpleDescReq, 5), IOutgoingPacket
{
    /// <summary>
    /// Specifies NWK address of the device generating the inquiry.
    /// </summary>
    public ushort DestAddr { get; } = destAddr;

    /// <summary>
    /// Specifies NWK address of the destination device being queried.
    /// </summary>
    public ushort NwkAddrOfInterest { get; } = nwkAddrOfInterest;

    /// <summary>
    /// Specifies the application endpoint the data is from.
    /// </summary>
    public byte Endpoint { get; } = endpoint;

    protected override byte[] Data { get; } = [.. BitConverter.GetBytes(destAddr), .. BitConverter.GetBytes(nwkAddrOfInterest), endpoint];
}
