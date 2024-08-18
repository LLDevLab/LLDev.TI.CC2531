using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;

public interface IAfDataResponse : IIncomingPacket
{
    ZToolPacketStatus Status { get; }
}

public sealed class AfDataResponse : IncomingPacket, IAfDataResponse
{
    public ZToolPacketStatus Status { get; }

    public AfDataResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x01)
    {
        Status = (ZToolPacketStatus)Data[0];
    }
}
