using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;

internal interface IZdoIeeeAddrResponse : IIncomingPacket
{
    public ZToolPacketStatus Status { get; }
}

internal sealed class ZdoIeeeAddrResponse : IncomingPacket, IZdoIeeeAddrResponse
{
    public ZToolPacketStatus Status { get; }

    public ZdoIeeeAddrResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x01)
    {
        Status = (ZToolPacketStatus)Data[0];
    }
}
