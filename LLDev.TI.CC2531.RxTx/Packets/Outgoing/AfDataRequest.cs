﻿using LLDev.TI.CC2531.RxTx.Enums;
using System.Buffers.Binary;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
internal sealed class AfDataRequest : OutgoingPacket, IOutgoingPacket
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
        dataList.AddRange(GetReversedEndianBytes(nwkDstAddr));
        dataList.Add(dstEndpoint);
        dataList.Add(srcEndpoint);
        dataList.AddRange(GetReversedEndianBytes((ushort)clusterId));
        dataList.Add(transId);
        dataList.Add(options);
        dataList.Add(radius);
        dataList.Add((byte)requestDataLen);
        dataList.AddRange(requestData);

        Data = [.. dataList];

        TransactionId = transId;
    }

    private static byte[] GetReversedEndianBytes(ushort value) =>
        BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(value));
}
