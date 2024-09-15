using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Incoming;

internal interface IAfRegisterResponse : IIncomingPacket
{
    public ZToolPacketStatus Status { get; }
}

internal sealed class AfRegisterResponse : IncomingPacket, IAfRegisterResponse
{
    public ZToolPacketStatus Status { get; }

    public AfRegisterResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x01)
    {
        Status = (ZToolPacketStatus)Data[0];
    }
}
