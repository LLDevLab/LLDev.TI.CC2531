namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;

internal interface IZdoExtFindGroupResponse : IIncomingPacket
{
    public byte[] GroupId { get; }
}

internal sealed class ZdoExtFindGroupResponse : IncomingPacket, IZdoExtFindGroupResponse
{
    public byte[] GroupId { get; }

    public ZdoExtFindGroupResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x00)
    {
        GroupId = Data;
    }
}
