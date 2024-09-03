using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Models;
using LLDev.TI.CC2531.RxTx.Packets.Incoming;
using LLDev.TI.CC2531.RxTx.Packets.Outgoing;
using LLDev.TI.CC2531.RxTx.Services;

namespace LLDev.TI.CC2531.RxTx.Devices;

internal interface INetworkCoordinator
{
    bool PingDevice();
    void ResetDevice();
    DeviceInfo GetDeviceInfo();
    bool StartupNetwork(ushort startupDelay);
    bool SetDeviceLedMode(byte ledId, bool isLedOn);
    bool PermitNetworkJoin(bool isPermited);
}

internal sealed class NetworkCoordinator(IPacketReceiverTransmitterService packetReceiverTransmitterService,
    ITransactionService transactionService) : INetworkCoordinator
{
    private readonly IPacketReceiverTransmitterService _packetReceiverTransmitterService = packetReceiverTransmitterService;
    private readonly ITransactionService _transactionService = transactionService;

    public DeviceInfo GetDeviceInfo()
    {
        var response = _packetReceiverTransmitterService.SendAndWaitForResponse<UtilGetDeviceInfoResponse>(new UtilGetDeviceInfoRequest(), ZToolCmdType.UtilGetDeviceInfoRsp);

        return response?.Status != ZToolPacketStatus.Success
            ? throw new NetworkException("Cannot receive network coordinator info")
            : new()
            {
                IeeeAddr = response.IeeeAddr,
                NwkAddr = response.NwkAddr
            };
    }

    public bool PermitNetworkJoin(bool isPermited)
    {
        var transactionId = _transactionService.GetNextTransactionId();

        var response = _packetReceiverTransmitterService.SendAndWaitForResponse<AfDataResponse>(new AfDataRequest(0,
            0,
            0,
            ZigBeeClusterId.PermitJoin,
            transactionId,
            0x30,
            31,
            3,
            [transactionId, (byte)(isPermited ? 255 : 0), 1]), ZToolCmdType.AfDataRsp);

        return response?.Status == ZToolPacketStatus.Success;
    }

    public bool PingDevice()
    {
        var response = _packetReceiverTransmitterService.SendAndWaitForResponse<SysPingResponse>(new SysPingRequest(), ZToolCmdType.SysPingRsp);

        return response is not null;
    }

    public void ResetDevice() => _packetReceiverTransmitterService.SendAndWaitForResponse<SysResetIndCallback>(new SysResetRequest(ZToolSysResetType.SerialBootloader), ZToolCmdType.SysResetIndClbk);

    public bool SetDeviceLedMode(byte ledId, bool isLedOn)
    {
        var response = _packetReceiverTransmitterService.SendAndWaitForResponse<UtilLedControlResponse>(new UtilLedControlRequest(ledId, isLedOn), ZToolCmdType.UtilLedControlRsp);

        return response?.Status == ZToolPacketStatus.Success;
    }

    public bool StartupNetwork(ushort startupDelay)
    {
        var response = _packetReceiverTransmitterService.SendAndWaitForResponse<ZdoStartupFromAppResponse>(new ZdoStartupFromAppRequest(startupDelay), ZToolCmdType.ZdoStartupFromAppRsp);

        return response?.Status is ZToolZdoStartupFromAppStatus.NewNetworkState or ZToolZdoStartupFromAppStatus.RestoredNwkState;
    }
}
