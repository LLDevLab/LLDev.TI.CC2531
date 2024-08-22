using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
public sealed class ZbReadConfigRequest : OutgoingPacket, IOutgoingPacket
{
    public ZToolZbConfigurationId ConfigId { get; }
    protected override byte[] Data { get; }

    public ZbReadConfigRequest(ZToolZbConfigurationId configId) :
        base(ZToolCmdType.ZbReadConfigurationReq, 1)
    {
        ConfigId = configId;
        Data = [(byte)ConfigId];
    }
}
