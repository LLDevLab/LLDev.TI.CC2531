using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Incoming;

internal interface IUtilLedControlResponse : IIncomingPacket
{
    public ZToolPacketStatus Status { get; }
}

internal sealed class UtilLedControlResponse : IncomingPacket, IUtilLedControlResponse
{
    public ZToolPacketStatus Status { get; }
    public UtilLedControlResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x01)
    {
        Status = (ZToolPacketStatus)Data[0];
    }
}
