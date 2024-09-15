using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Incoming;

internal interface IZdoSimpleDescCallback : IIncomingPacket
{
    public ushort SrcAddr { get; }
    public ZToolPacketStatus Status { get; }
    public ushort NwkAddr { get; }
    public byte Len { get; }
    public byte Endpoint { get; }
    public ushort ProfileId { get; }
    public ushort DeviceId { get; }
    public byte DeviceVersion { get; }
    public byte NumInClusters { get; }
    public ushort[] InClusters { get; }
    public byte NumOutClusters { get; }
    public ushort[] OutClusters { get; }
}

internal sealed class ZdoSimpleDescCallback : IncomingPacket, IZdoSimpleDescCallback
{
    /// <summary>
    /// Specifies the message’s source network address.
    /// </summary>
    public ushort SrcAddr { get; }

    public ZToolPacketStatus Status { get; }

    /// <summary>
    /// Specifies Device’s short address that this response describes.
    /// </summary>
    public ushort NwkAddr { get; }

    /// <summary>
    /// Specifies the length of the simple descriptor
    /// </summary>
    public byte Len { get; }

    /// <summary>
    /// Specifies Endpoint of the device
    /// </summary>
    public byte Endpoint { get; }

    /// <summary>
    /// The profile Id for this endpoint.
    /// </summary>
    public ushort ProfileId { get; }

    /// <summary>
    /// The Device Description Id for this endpoint.
    /// </summary>
    public ushort DeviceId { get; }

    /// <summary>
    /// Defined as the following format 0 – Version 1.00, 0x01-0x0F – Reserved
    /// </summary>
    public byte DeviceVersion { get; }

    /// <summary>
    /// The number of input clusters in the InClusterList.
    /// </summary>
    public byte NumInClusters { get; }

    /// <summary>
    /// List of input cluster Id’s supported.
    /// </summary>
    public ushort[] InClusters { get; }

    /// <summary>
    /// The number of output clusters in the OutClusterList.
    /// </summary>
    public byte NumOutClusters { get; }

    /// <summary>
    /// List of output cluster Id’s supported.
    /// </summary>
    public ushort[] OutClusters { get; }

    public ZdoSimpleDescCallback(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x0e)
    {
        SrcAddr = GetUShort(Data[1], Data[0]);
        Status = (ZToolPacketStatus)Data[2];
        NwkAddr = GetUShort(Data[4], Data[3]);
        Len = Data[5];
        Endpoint = Data[6];
        ProfileId = GetUShort(Data[8], Data[7]);
        DeviceId = GetUShort(Data[10], Data[9]);
        DeviceVersion = Data[11];
        NumInClusters = Data[12];

        var idx = 13;
        InClusters = GetClusters(NumInClusters, idx);
        idx += NumInClusters * 2;
        NumOutClusters = Data[idx];
        idx++;
        OutClusters = GetClusters(NumOutClusters, idx);
    }

    private ushort[] GetClusters(int numClusters, int offset)
    {
        var clusters = new ushort[numClusters];

        for (var i = 0; i < numClusters; i++)
            clusters[i] = GetUShort(Data[offset + i + 1], Data[offset + i]);

        return clusters;
    }
}
