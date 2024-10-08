﻿using LLDev.TI.CC2531.Enums;
using System.Buffers.Binary;

namespace LLDev.TI.CC2531.Packets.Outgoing;
internal sealed class ZdoNwkDiscoveryRequest : OutgoingPacket, IOutgoingPacket
{
    public ZToolDiscoveryScanChannelTypes ChannelTypes { get; }
    public byte ScanDuration { get; }

    protected override byte[] Data { get; }

    public ZdoNwkDiscoveryRequest(ZToolDiscoveryScanChannelTypes channelTypes, byte scanDuration) :
        base(ZToolCmdType.ZdoNwkDiscoveryReq, 0x05)
    {
        ChannelTypes = channelTypes;
        ScanDuration = scanDuration;

        var dataList = new List<byte>();
        dataList.AddRange(BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((uint)channelTypes)));
        dataList.Add(ScanDuration);
        Data = [.. dataList];
    }
}
