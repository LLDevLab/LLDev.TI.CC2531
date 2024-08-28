namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;

internal sealed class SysGetExtAddrResponse : IncomingPacket, IIncomingPacket
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
