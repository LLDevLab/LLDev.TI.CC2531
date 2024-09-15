using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Incoming;

internal interface IZdoMsgCbRegisterResponse : IIncomingPacket
{
    public ZToolPacketStatus Status { get; }
}

internal sealed class ZdoMsgCbRegisterResponse : IncomingPacket, IZdoMsgCbRegisterResponse
{
    public ZToolPacketStatus Status { get; }
    public ZdoMsgCbRegisterResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x01)
    {
        Status = (ZToolPacketStatus)Data[0];
    }
}
