namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;
internal sealed class ZdoExtFindGroupResponse : IncomingPacket, IIncomingPacket
{
    public byte[] GroupId { get; }

    public ZdoExtFindGroupResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x00)
    {
        GroupId = Data;
    }
}
