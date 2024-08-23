using LLDev.TI.CC2531.RxTx.Enums;
using System.Buffers.Binary;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
public sealed class ZdoNwkDiscoveryRequest : OutgoingPacket, IOutgoingPacket
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
