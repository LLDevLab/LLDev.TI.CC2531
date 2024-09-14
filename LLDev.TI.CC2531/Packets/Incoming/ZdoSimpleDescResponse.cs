using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;

internal interface IZdoSimpleDescResponse : IIncomingPacket
{
    public ZToolPacketStatus Status { get; }
}

internal sealed class ZdoSimpleDescResponse : IncomingPacket, IZdoSimpleDescResponse
{
    public ZToolPacketStatus Status { get; }

    public ZdoSimpleDescResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x01)
    {
        Status = (ZToolPacketStatus)Data[0];
    }
}
