using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets.Outgoing;
internal sealed class ZbWriteConfigRequest : OutgoingPacket, IOutgoingPacket
{
    public ZToolZbConfigurationId ConfigId { get; }
    public byte ConfigDataLen { get; }
    public byte[] ConfigData { get; }

    protected override byte[] Data { get; }

    public ZbWriteConfigRequest(ZToolZbConfigurationId configId, byte[] data) :
        base(ZToolCmdType.ZbWriteConfigurationReq, (byte)(2 + data.Length))
    {
        ConfigId = configId;
        ConfigDataLen = (byte)data.Length;
        ConfigData = data;
        var dataList = new List<byte>
        {
            (byte)ConfigId,
            ConfigDataLen
        };
        dataList.AddRange(ConfigData);
        Data = [.. dataList];
    }
}
