namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;

public sealed class SysVersionResponse : IncomingPacket, IIncomingPacket
{
    public byte TransportRev { get; }
    public byte ProductId { get; }
    public byte MajorRel { get; }
    public byte MinorRel { get; }
    public byte MaintRel { get; }

    public SysVersionResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x05)
    {
        TransportRev = Data[0];
        ProductId = Data[1];
        MajorRel = Data[2];
        MinorRel = Data[3];
        MaintRel = Data[4];
    }
}
