using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Extensions;
using LLDev.TI.CC2531.RxTx.Packets.Incoming;
using Microsoft.Extensions.Logging;

namespace LLDev.TI.CC2531.RxTx.Packets;

public interface IPacketFactory
{
    IncomingPacket? CreateIncomingPacket(byte[] packet);
}

public sealed class PacketFactory(ILogger<IPacketFactory> logger) : IPacketFactory
{
    private readonly ILogger<IPacketFactory> _logger = logger;

    public IncomingPacket? CreateIncomingPacket(byte[] packet)
    {
        IncomingPacket? result = null;

        try
        {
            var packetHeader = new PacketHeader(packet);

            result = packetHeader.CmdType switch
            {
                ZToolCmdType.SysResetIndClbk => new SysResetIndCallback(packetHeader, packet),
                ZToolCmdType.ZdoEndDeviceAnnceIndClbk => new ZdoEndDeviceAnnceIndCallback(packetHeader, packet),
                ZToolCmdType.ZdoMsgCbIncomingClbk => new ZdoMsgCbIncomingCallback(packetHeader, packet),
                ZToolCmdType.SysVersionRsp => new SysVersionResponse(packetHeader, packet),
                ZToolCmdType.ZdoMsgCbRegisterRsp => new ZdoMsgCbRegisterResponse(packetHeader, packet),
                ZToolCmdType.ZdoStartupFromAppRsp => new ZdoStartupFromAppResponse(packetHeader, packet),
                ZToolCmdType.ZbReadConfigurationRsp => new ZbReadConfigResponse(packetHeader, packet),
                ZToolCmdType.ZbWriteConfigurationRsp => new ZbWriteConfigResponse(packetHeader, packet),
                ZToolCmdType.ZbGetDeviceInfoRsp => new ZbGetDeviceInfoResponse(packetHeader, packet),
                ZToolCmdType.UtilLedControlRsp => new UtilLedControlResponse(packetHeader, packet),
                ZToolCmdType.ZdoNodeDescRsp => new ZdoNodeDescCallback(packetHeader, packet),
                ZToolCmdType.ZdoNodeDescClbk => new ZdoNodeDescResponse(packetHeader, packet),
                ZToolCmdType.AfDataRsp => new AfDataResponse(packetHeader, packet),
                ZToolCmdType.ZdoNwkAddrClbk => new ZdoNwkAddrCallback(packetHeader, packet),
                ZToolCmdType.ZdoStateChangedIndClbk => new ZdoStateChangedIndCallback(packetHeader, packet),
                ZToolCmdType.ZdoNwkDiscoveryRsp => new ZdoNwkDiscoveryResponse(packetHeader, packet),
                ZToolCmdType.SysPingRsp => new SysPingResponse(packetHeader, packet),
                ZToolCmdType.ZdoActiveEpRsp => new ZdoActiveEpResponse(packetHeader, packet),
                ZToolCmdType.ZdoActiveEpClbk => new ZdoActiveEpCallback(packetHeader, packet),
                ZToolCmdType.AfRegisterRsp => new AfRegisterResponse(packetHeader, packet),
                ZToolCmdType.UtilGetDeviceInfoRsp => new UtilGetDeviceInfoResponse(packetHeader, packet),
                ZToolCmdType.ZdoExtRouteDiscRsp => new ZdoExtRouteDiscoveryResponse(packetHeader, packet),
                ZToolCmdType.SysGetExtAddrRsp => new SysGetExtAddrResponse(packetHeader, packet),
                ZToolCmdType.SysOsalNvLengthRsp => new SysOsalNvLengthResponse(packetHeader, packet),
                ZToolCmdType.SysOsalNvReadRsp => new SysOsalNvReadResponse(packetHeader, packet),
                ZToolCmdType.ZdoExtFindGroupRsp => new ZdoExtFindGroupResponse(packetHeader, packet),
                ZToolCmdType.ZdoSimpleDescRsp => new ZdoSimpleDescResponse(packetHeader, packet),
                ZToolCmdType.ZdoSimpleDescClbk => new ZdoSimpleDescCallback(packetHeader, packet),
                ZToolCmdType.ZdoExtNwkInfoRsp => new ZdoExtNwkInfoResponse(packetHeader, packet),
                ZToolCmdType.AfIncomingMsgClbk => new AfIncomingMessageCallback(packetHeader, packet),
                ZToolCmdType.ZdoIeeeAddrRsp => new ZdoIeeeAddrResponse(packetHeader, packet),
                ZToolCmdType.ZdoIeeeAddrClbk => new ZdoIeeeAddrCallback(packetHeader, packet),
                _ => throw new PacketException("Unsupported packet type")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("Cannot create network packet instance. Packet: '{Packet}', exception: '{Exception}'", packet.ArrayToString(), ex.Message);
        }

        return result;
    }
}
