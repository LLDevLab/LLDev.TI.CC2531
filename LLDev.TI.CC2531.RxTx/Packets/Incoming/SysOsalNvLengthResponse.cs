namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;

internal sealed class SysOsalNvLengthResponse : IncomingPacket, IIncomingPacket
{
    /// <summary>
    /// 0x0000 = item does not exist, 0x0001-0xNNNN = number of bytes in NV item
    /// </summary>
    public ushort Length { get; }

    public SysOsalNvLengthResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x02)
    {
        Length = GetUShort(Data[1], Data[0]);
    }
}
