using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;

public sealed class SysResetIndCallback : IncomingPacket, IIncomingPacket
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
