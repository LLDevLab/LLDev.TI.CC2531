using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Incoming;

internal interface IZdoActiveEpCallback : IIncomingPacket
{
    public ushort SrcAddr { get; }
    public ZToolPacketStatus Status { get; }
    public ushort NwkAddr { get; }
    public byte ActiveEpCount { get; }
    public byte[] ActiveEps { get; }
}

internal sealed class ZdoActiveEpCallback : IncomingPacket, IZdoActiveEpCallback
{
    /// <summary>
    /// The message’s source network address.
    /// </summary>
    public ushort SrcAddr { get; }

    /// <summary>
    /// This field indicates either SUCCESS or FAILURE.
    /// </summary>
    public ZToolPacketStatus Status { get; }

    /// <summary>
    /// Device’s short address that this response describes.
    /// </summary>
    public ushort NwkAddr { get; }

    /// <summary>
    /// Number of active endpoint in the list
    /// </summary>
    public byte ActiveEpCount { get; }

    /// <summary>
    /// Array of active endpoints on this device.
    /// </summary>
    public byte[] ActiveEps { get; }

    public ZdoActiveEpCallback(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x06)
    {
        SrcAddr = GetUShort(Data[0], Data[1]);
        Status = (ZToolPacketStatus)Data[2];
        NwkAddr = GetUShort(Data[3], Data[4]);
        ActiveEpCount = Data[5];
        ActiveEps = Data[6..];
    }
}
