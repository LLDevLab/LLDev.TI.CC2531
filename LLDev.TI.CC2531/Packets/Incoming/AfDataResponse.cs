using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Incoming;

internal interface IAfDataResponse : IIncomingPacket
{
    public ZToolPacketStatus Status { get; }
}

internal sealed class AfDataResponse : IncomingPacket, IAfDataResponse
{
    public ZToolPacketStatus Status { get; }

    public AfDataResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x01)
    {
        Status = (ZToolPacketStatus)Data[0];
    }
}
