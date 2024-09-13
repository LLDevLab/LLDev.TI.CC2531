using LLDev.TI.CC2531.RxTx.Enums;
using System.Buffers.Binary;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
internal sealed class AfDataRequest : OutgoingPacket, IOutgoingPacket
{
    public ushort NwkDstAddr { get; }
    public byte DstEndpoint { get; }
    public byte SrcEndpoint { get; }
    public ZigBeeClusterId ClusterId { get; }
    public byte TransactionId { get; }
    public byte Options { get; }
    public byte Radius { get; }
    public sbyte RequestDataLen { get; }
    public byte[] RequestData { get; }

    protected sealed override byte[] Data { get; }

    public AfDataRequest(ushort nwkDstAddr,
        byte dstEndpoint,
        byte srcEndpoint,
        ZigBeeClusterId clusterId,
        byte transId,
        byte options,
        byte radius,
        sbyte requestDataLen,
        byte[] requestData) : base(ZToolCmdType.AfDataReq, (byte)(10 + requestDataLen))
    {
        NwkDstAddr = nwkDstAddr;
        DstEndpoint = dstEndpoint;
        SrcEndpoint = srcEndpoint;
        ClusterId = clusterId;
        TransactionId = transId;
        Options = options;
        Radius = radius;
        RequestDataLen = requestDataLen;
        RequestData = requestData;

        var dataList = new List<byte>();
        dataList.AddRange(GetReversedEndianBytes(NwkDstAddr));
        dataList.Add(DstEndpoint);
        dataList.Add(SrcEndpoint);
        dataList.AddRange(GetReversedEndianBytes((ushort)ClusterId));
        dataList.Add(TransactionId);
        dataList.Add(Options);
        dataList.Add(Radius);
        dataList.Add((byte)RequestDataLen);
        dataList.AddRange(RequestData);

        Data = [.. dataList];
    }

    private static byte[] GetReversedEndianBytes(ushort value) =>
        BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(value));
}
