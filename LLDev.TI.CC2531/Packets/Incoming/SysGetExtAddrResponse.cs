namespace LLDev.TI.CC2531.Packets.Incoming;

internal interface ISysGetExtAddrResponse : IIncomingPacket
{
    public ulong ExtAddress { get; }
}

internal sealed class SysGetExtAddrResponse : IncomingPacket, ISysGetExtAddrResponse
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
