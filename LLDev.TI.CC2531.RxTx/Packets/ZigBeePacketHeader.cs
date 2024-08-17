using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets;

public interface IZigBeePacketHeader
{
    byte StartByte { get; }
    byte DataLength { get; }
    ZToolCmdType CmdType { get; }

    public byte[] ToByteArray();
}

public sealed class ZigBeePacketHeader(byte[] data) : IZigBeePacketHeader
{
    public byte StartByte => _data[0];
    public byte DataLength => _data[1];
    public ZToolCmdType CmdType
    {
        get
        {
            var cmd = (ushort)((_data[2] << 8) | _data[3]);
            return Enum.IsDefined(typeof(ZToolCmdType), cmd) ? (ZToolCmdType)cmd : ZToolCmdType.Unknown;
        }
    }

    private readonly byte[] _data = data[0..4];

    public byte[] ToByteArray() => _data;
}
