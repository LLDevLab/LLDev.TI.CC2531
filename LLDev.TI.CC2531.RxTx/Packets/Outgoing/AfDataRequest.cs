using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
public sealed class AfDataRequest : OutgoingPacket
{
    public byte TransactionId { get; }

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
        var dataList = new List<byte>();
        dataList.AddRange(GetBytesFromUshort(nwkDstAddr));
        dataList.Add(dstEndpoint);
        dataList.Add(srcEndpoint);
        dataList.AddRange(GetBytesFromUshort((ushort)clusterId));
        dataList.Add(transId);
        dataList.Add(options);
        dataList.Add(radius);
        dataList.Add((byte)requestDataLen);
        dataList.AddRange(requestData);

        Data = [.. dataList];

        TransactionId = transId;
    }

    private static byte[] GetBytesFromUshort(ushort value)
    {
        var bytes = new byte[2];
        bytes[0] = (byte)(value >> 8);
        bytes[1] = (byte)value;
        return bytes;
    }
}
