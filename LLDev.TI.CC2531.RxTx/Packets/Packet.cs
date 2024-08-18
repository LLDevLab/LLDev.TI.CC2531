﻿using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Extensions;

namespace LLDev.TI.CC2531.RxTx.Packets;

public interface IPacket
{
    byte DataLength { get; }
    ZToolCmdType CmdType { get; }

    byte[] ToByteArray();
}

public abstract class Packet(IPacketHeader packetHeader) : IPacket
{
    protected const byte StartByte = 0xfe;
    public byte DataLength => PacketHeader.DataLength;
    public ZToolCmdType CmdType => PacketHeader.CmdType;

    protected IPacketHeader PacketHeader { get; } = packetHeader;

    protected abstract byte[] Data { get; }

    public byte[] ToByteArray()
    {
        var resultList = new List<byte>();
        resultList.AddRange(PacketHeader.ToByteArray());
        resultList.AddRange(Data);
        resultList.Add(CalcCheckSum());
        return [.. resultList];
    }

    protected byte CalcCheckSum()
    {
        byte checkSum = 0;
        var headerData = PacketHeader.ToByteArray();

        // First item in the packet is a start bite, it should not be added to a checksum
        for (var i = 1; i < headerData.Length; i++)
            checkSum = (byte)(checkSum ^ headerData[i]);

        foreach (var dataItem in Data)
            checkSum = (byte)(checkSum ^ dataItem);

        return checkSum;
    }

    public sealed override string ToString() => $"Packet type {CmdType}; Data: {Data.ArrayToString()}";
}