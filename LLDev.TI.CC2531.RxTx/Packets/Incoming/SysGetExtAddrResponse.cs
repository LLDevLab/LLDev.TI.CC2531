namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;

public interface ISysGetExtAddrResponse : IIncomingPacket
{
    ulong ExtAddress { get; }
}

public sealed class SysGetExtAddrResponse : IncomingPacket, ISysGetExtAddrResponse
{
    /// <summary>
    /// The device’s extended address.
    /// </summary>
    public ulong ExtAddress { get; }

    public SysGetExtAddrResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x08)
    {
        ExtAddress = GetBigEndianULong(Data);
    }
}
