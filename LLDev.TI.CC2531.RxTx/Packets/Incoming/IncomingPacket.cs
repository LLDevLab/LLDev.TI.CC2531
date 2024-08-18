﻿namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;

public interface IIncomingPacket
{
    bool IsPacketCorrect { get; }
    bool IsCheckSumCorrect { get; }
    bool IsStartByteCorrect { get; }
    bool IsCorrectPacketFormat { get; }
}

public class IncomingPacket : Packet, IIncomingPacket
{
    public bool IsPacketCorrect => IsStartByteCorrect && IsCheckSumCorrect && IsCorrectPacketFormat;
    public bool IsCheckSumCorrect => _checkSum == _calcCheckSum;
    public bool IsStartByteCorrect => PacketHeader.StartByte == StartByte;
    public bool IsCorrectPacketFormat => PacketHeader.DataLength >= _minDataLength;

    /// <summary>
    /// Actual data of a packet
    /// </summary>
    protected sealed override byte[] Data { get; }

    private readonly byte _checkSum;
    private readonly byte _calcCheckSum;
    private readonly byte _minDataLength;

    protected IncomingPacket(IPacketHeader packetHeader, byte[] packet, byte minDataLength) :
        base(packetHeader)
    {
        Data = packet[..^1];
        _checkSum = packet[^1];
        _calcCheckSum = CalcCheckSum();
        _minDataLength = minDataLength;
    }

    protected static ushort GetUShort(byte msb, byte lsb) => (ushort)((msb << 8) | lsb);

    protected static ulong GetBigEndianULong(byte[] data)
    {
        ulong result = 0;

        for (var i = data.Length - 1; i >= 0; i--)
        {
            result <<= 8;
            result |= data[i];
        }

        return result;
    }

    protected static ulong GetLittleEndianULong(byte[] data)
    {
        ulong result = 0;

        for (var i = 0; i < data.Length; i++)
        {
            result <<= 8;
            result |= data[i];
        }

        return result;
    }
}