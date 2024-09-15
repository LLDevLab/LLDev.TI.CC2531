using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Incoming;

internal interface IZdoExtNwkInfoResponse : IIncomingPacket
{
    public ushort ShortAddr { get; }
    public ZToolZdoState DevState { get; }
    public ushort PanId { get; }
    public ushort ParentAddr { get; }
    public ulong ExtendedPanId { get; }
    public ulong ExtendedParentAddr { get; }
    public byte Channel { get; }
}

internal sealed class ZdoExtNwkInfoResponse : IncomingPacket, IZdoExtNwkInfoResponse
{
    public ushort ShortAddr { get; }
    public ZToolZdoState DevState { get; }
    public ushort PanId { get; }
    public ushort ParentAddr { get; }
    public ulong ExtendedPanId { get; }

    /// <summary>
    /// IEEE address of parent
    /// </summary>
    public ulong ExtendedParentAddr { get; }

    public byte Channel { get; }

    public ZdoExtNwkInfoResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x18)
    {
        ShortAddr = GetUShort(Data[1], Data[0]);
        DevState = (ZToolZdoState)Data[2];
        PanId = GetUShort(Data[4], Data[3]);
        ParentAddr = GetUShort(Data[6], Data[5]);
        ExtendedPanId = GetBigEndianULong(Data[7..15]);
        ExtendedParentAddr = GetBigEndianULong(Data[15..23]);
        Channel = Data[23];
    }
}
