﻿using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;
public sealed class ZdoNwkAddrResponse : IncomingPacket, IIncomingPacket
{
    /// <summary>
    /// This field indicates either SUCCESS or FAILURE.
    /// </summary>
    public ZToolPacketStatus Status { get; }

    /// <summary>
    /// 64 bit IEEE address of source device.
    /// </summary>
    public ulong IeeeAddr { get; }

    /// <summary>
    /// Specifies the short network address of responding device.
    /// </summary>
    public ushort NwkAddr { get; }

    /// <summary>
    /// Specifies the starting index into the list of associated devices for this report.
    /// </summary>
    public byte StartIndex { get; }

    /// <summary>
    /// Specifies the number of associated devices.
    /// </summary>
    public byte NumAssocDev { get; }

    /// <summary>
    /// Contains the list of network address for associated devices. This list can be a partial list if the entire list doesn’t fit into a packet. If it is a partial list, the starting index is StartIndex.
    /// </summary>
    public IReadOnlyList<ushort> AssocDevList => _assocDevList;

    private readonly List<ushort> _assocDevList;

    public ZdoNwkAddrResponse(IPacketHeader header, byte[] packet) :
        base(header, packet, 0x0d)
    {
        _assocDevList = [];

        Status = (ZToolPacketStatus)Data[0];
        IeeeAddr = GetLittleEndianULong(Data[1..9]);
        NwkAddr = GetUShort(Data[9], Data[10]);
        StartIndex = Data[11];
        NumAssocDev = Data[12];

        var assocDevList = Data[13..];
        for (var i = 0; i < assocDevList.Length; i += 2)
            _assocDevList.Add(GetUShort(assocDevList[i], assocDevList[i + 1]));
    }
}