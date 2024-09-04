using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;

internal interface IZdoNodeDescResponse : IIncomingPacket
{
    public ZToolPacketStatus Status { get; }
}

internal sealed class ZdoNodeDescResponse : IncomingPacket, IZdoNodeDescResponse
{
    public ZToolPacketStatus Status { get; }
    public ZdoNodeDescResponse(IPacketHeader header, byte[] data) :
        base(header, data, 0x01)
    {
        Status = (ZToolPacketStatus)Data[0];
    }
}
