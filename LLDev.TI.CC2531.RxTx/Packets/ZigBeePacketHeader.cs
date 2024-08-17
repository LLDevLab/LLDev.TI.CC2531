using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets;

public interface IZigBeePacketHeader
{
    byte StartByte { get; }
    byte DataLength { get; }
    ZToolCmdType CmdType { get; }

    public byte[] ToByteArray();
}

public sealed class ZigBeePacketHeader : IZigBeePacketHeader
{
    public byte StartByte { get; }
    public byte DataLength { get; }
    public ZToolCmdType CmdType { get; }

    private readonly byte[] _data;

    public ZigBeePacketHeader(byte[] data)
    {
        _data = data[0..4];
        StartByte = _data[0];
        DataLength = _data[1];

        var cmd = (ushort)((_data[2] << 8) | _data[3]);
        CmdType = Enum.IsDefined(typeof(ZToolCmdType), cmd) ? (ZToolCmdType)cmd : ZToolCmdType.Unknown;
    }

    public byte[] ToByteArray() => _data;
}
