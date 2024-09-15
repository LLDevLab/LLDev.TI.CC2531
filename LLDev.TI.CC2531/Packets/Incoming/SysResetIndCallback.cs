using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Incoming;

internal interface ISysResetIndCallback : IIncomingPacket
{
    public ZToolDeviceResetReason Reason { get; }
    public byte TransportRev { get; }
    public byte ProductId { get; }
    public byte MajorRel { get; }
    public byte MinorRel { get; }
    public byte HwRev { get; }
}

internal sealed class SysResetIndCallback : IncomingPacket, ISysResetIndCallback
{
    /// <summary>
    /// Reason for the reset.
    /// </summary>
    public ZToolDeviceResetReason Reason { get; }

    /// <summary>
    /// Transport protocol revision.
    /// </summary>
    public byte TransportRev { get; }

    public byte ProductId { get; }

    /// <summary>
    /// Major release number.
    /// </summary>
    public byte MajorRel { get; }

    /// <summary>
    /// Minor release number.
    /// </summary>
    public byte MinorRel { get; }

    /// <summary>
    /// Hardware revision number.
    /// </summary>
    public byte HwRev { get; }

    public SysResetIndCallback(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x06)
    {
        Reason = (ZToolDeviceResetReason)Data[0];
        TransportRev = Data[1];
        ProductId = Data[2];
        MajorRel = Data[3];
        MinorRel = Data[4];
        HwRev = Data[5];
    }
}
