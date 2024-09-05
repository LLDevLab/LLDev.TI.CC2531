using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Models;
using LLDev.TI.CC2531.RxTx.Packets.Incoming;
using LLDev.TI.CC2531.RxTx.Packets.Outgoing;
using LLDev.TI.CC2531.RxTx.Services;

namespace LLDev.TI.CC2531.RxTx.Devices;

internal interface INetworkCoordinator
{
    void PingCoordinatorOrThrow();
    void ResetCoordinator();
    DeviceInfo GetCoordinatorInfo();
    bool StartupNetwork(ushort startupDelay);
    bool SetCoordinatorLedMode(byte ledId, bool isLedOn);
    bool PermitNetworkJoin(bool isPermited);
}

internal sealed class NetworkCoordinator(IPacketReceiverTransmitterService packetReceiverTransmitterService,
    ITransactionService transactionService) : INetworkCoordinator
{
    private readonly IPacketReceiverTransmitterService _packetReceiverTransmitterService = packetReceiverTransmitterService;
    private readonly ITransactionService _transactionService = transactionService;

    public DeviceInfo GetCoordinatorInfo()
    {
        var response = _packetReceiverTransmitterService.SendAndWaitForResponse<IUtilGetDeviceInfoResponse>(new UtilGetDeviceInfoRequest(), ZToolCmdType.UtilGetDeviceInfoRsp);

        return response.Status != ZToolPacketStatus.Success
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

        var response = _packetReceiverTransmitterService.SendAndWaitForResponse<IAfDataResponse>(new AfDataRequest(0,
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

    public void PingCoordinatorOrThrow()
    {
        try
        {
            _packetReceiverTransmitterService.SendAndWaitForResponse<ISysPingResponse>(new SysPingRequest(), ZToolCmdType.SysPingRsp);
        }
        catch (Exception e)
        {
            throw new NetworkException("Cannot ping network coordinator", e);
        }
    }

    public void ResetCoordinator() => _packetReceiverTransmitterService.SendAndWaitForResponse<ISysResetIndCallback>(new SysResetRequest(ZToolSysResetType.SerialBootloader), ZToolCmdType.SysResetIndClbk);

    public bool SetCoordinatorLedMode(byte ledId, bool isLedOn)
    {
        var response = _packetReceiverTransmitterService.SendAndWaitForResponse<IUtilLedControlResponse>(new UtilLedControlRequest(ledId, isLedOn), ZToolCmdType.UtilLedControlRsp);

        return response?.Status == ZToolPacketStatus.Success;
    }

    public bool StartupNetwork(ushort startupDelay)
    {
        var response = _packetReceiverTransmitterService.SendAndWaitForResponse<IZdoStartupFromAppResponse>(new ZdoStartupFromAppRequest(startupDelay), ZToolCmdType.ZdoStartupFromAppRsp);

        return response?.Status is ZToolZdoStartupFromAppStatus.NewNetworkState or ZToolZdoStartupFromAppStatus.RestoredNwkState;
    }
}
