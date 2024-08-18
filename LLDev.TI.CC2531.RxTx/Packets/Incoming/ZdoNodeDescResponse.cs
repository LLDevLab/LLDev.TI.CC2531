using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;

/// <summary>
/// This callback message is in response to the ZDO Node Descriptor Request.
/// </summary>
public sealed class ZdoNodeDescResponse : IncomingPacket, IIncomingPacket
{
    /// <summary>
    /// The message’s source network address.
    /// </summary>
    public ushort SrcAddr { get; }
    public ZToolPacketStatus Status { get; }

    /// <summary>
    /// Device’s short address of this Node descriptor.
    /// </summary>
    public ushort NwkAddrOfInterest { get; }

    public ZigBeeDeviceType DeviceType { get; }
    public bool IsComplexDescriptorAvailable { get; }
    public bool IsUserDescriptorAvailable { get; }

    /// <summary>
    /// Specifies a manufacturer code that is allocated by the ZigBee Alliance, relating to the manufacturer to the device.
    /// </summary>
    public ushort ManufacturerCode { get; }

    /// <summary>
    /// Indicates size of maximum NPDU. This field is used as a high level indication for management.
    /// </summary>
    public byte MaxBufferSize { get; }

    /// <summary>
    /// Indicates maximum size of Transfer up to 0x7fff (This field is reserved in version 1.0 and shall be set to zero).
    /// </summary>
    public ushort MaxTransferSize { get; }
    public bool IsPrimaryTrustCenter => _serverMask.HasFlag(ZToolZdoNodeDescRpsServerMask.PrimaryTrustCenter);
    public bool IsBackupTrustCenter => _serverMask.HasFlag(ZToolZdoNodeDescRpsServerMask.BackupTrustCenter);
    public bool IsPrimaryBindTableCache => _serverMask.HasFlag(ZToolZdoNodeDescRpsServerMask.PrimaryBindTableCache);
    public bool IsBackupBindTableCache => _serverMask.HasFlag(ZToolZdoNodeDescRpsServerMask.BackupBindTableCache);
    public bool IsPrimaryDiscoveryCache => _serverMask.HasFlag(ZToolZdoNodeDescRpsServerMask.PrimaryDiscoveryCache);
    public bool IsBackupDiscoveryCache => _serverMask.HasFlag(ZToolZdoNodeDescRpsServerMask.BackupDiscoveryCache);

    /// <summary>
    /// Indicates maximum size of Transfer up to 0x7fff (This field is reserved in version 1.0 and shall be set to zero). 
    /// </summary>
    public ushort MaxOutTransferSize { get; }

    private readonly ZToolZdoNodeDescRpsServerMask _serverMask;

    public ZdoNodeDescResponse(IPacketHeader header, byte[] data) :
        base(header, data, 0x12)
    {
        SrcAddr = GetUShort(Data[1], Data[0]);
        Status = (ZToolPacketStatus)Data[2];
        NwkAddrOfInterest = GetUShort(Data[4], Data[3]);

        // Take only 3 first bits of the byte
        DeviceType = (ZigBeeDeviceType)(byte)(Data[5] & 0x07);
        IsComplexDescriptorAvailable = (Data[5] & 0x08) != 0;
        IsUserDescriptorAvailable = (Data[5] & 0x10) != 0;
        ManufacturerCode = GetUShort(Data[9], Data[8]);
        MaxBufferSize = Data[10];
        MaxTransferSize = GetUShort(Data[12], Data[11]);
        _serverMask = (ZToolZdoNodeDescRpsServerMask)GetUShort(Data[14], Data[13]);
        MaxOutTransferSize = GetUShort(Data[16], Data[15]);
    }
}
