namespace LLDev.TI.CC2531.RxTx.Packets.Incoming;

internal interface IIncomingPacket
{
    bool IsPacketCorrect { get; }
    bool IsCheckSumCorrect { get; }
    bool IsStartByteCorrect { get; }
    bool IsCorrectPacketFormat { get; }
}

internal class IncomingPacket : Packet, IIncomingPacket
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

    protected IncomingPacket(IPacketHeader header, byte[] packet, byte minDataLength) :
        base(header)
    {
        Data = packet[..^1];
        _checkSum = packet[^1];
        _calcCheckSum = CalcCheckSum();
        _minDataLength = minDataLength;
    }

    protected static ushort GetUShort(byte msb, byte lsb) => BitConverter.ToUInt16([lsb, msb]);

    protected static ulong GetBigEndianULong(byte[] data)
    {
        // Since BitConverter.ToUInt64 requires no less that 4 byte array, cannot use it here
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
        // Since BitConverter.ToUInt64 requires no less that 4 byte array, cannot use it here
        ulong result = 0;

        for (var i = 0; i < data.Length; i++)
        {
            result <<= 8;
            result |= data[i];
        }

        return result;
    }
}
