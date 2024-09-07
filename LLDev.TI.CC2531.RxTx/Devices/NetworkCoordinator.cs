using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Models;
using LLDev.TI.CC2531.RxTx.Packets.Incoming;
using LLDev.TI.CC2531.RxTx.Packets.Outgoing;
using LLDev.TI.CC2531.RxTx.Services;
using Microsoft.Extensions.Logging;

namespace LLDev.TI.CC2531.RxTx.Devices;

internal interface INetworkCoordinator
{
    void PingCoordinatorOrThrow();
    void ResetCoordinator();
    DeviceInfo GetCoordinatorInfo();
    bool StartupNetwork(ushort startupDelay);
    bool SetCoordinatorLedMode(byte ledId, bool isLedOn);
    bool PermitNetworkJoin(bool isPermited);
    List<NetworkEndpoint> GetSupportedEndpoints();
    byte[] GetActiveEndpointIds();
    void ValidateRegisteredEndpoints(IEnumerable<byte> endpointIds);
    void RegisterEndpoints(IEnumerable<NetworkEndpoint> endpoints);
}

internal sealed class NetworkCoordinator(IPacketReceiverTransmitterService packetReceiverTransmitterService,
    ITransactionService transactionService,
    ILogger<NetworkCoordinator> logger) : INetworkCoordinator
{
    private readonly IPacketReceiverTransmitterService _packetReceiverTransmitterService = packetReceiverTransmitterService;
    private readonly ITransactionService _transactionService = transactionService;
    private readonly ILogger<NetworkCoordinator> _logger = logger;

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

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Trying to set network permit join to {IsPermited}. Response status {Status}.", isPermited, response.Status);

        return response.Status == ZToolPacketStatus.Success;
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

    public void ResetCoordinator()
    {
        _packetReceiverTransmitterService.SendAndWaitForResponse<ISysResetIndCallback>(new SysResetRequest(ZToolSysResetType.SerialBootloader), ZToolCmdType.SysResetIndClbk);

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Coordinator device have been resetted");
    }

    public bool SetCoordinatorLedMode(byte ledId, bool isLedOn)
    {
        var response = _packetReceiverTransmitterService.SendAndWaitForResponse<IUtilLedControlResponse>(new UtilLedControlRequest(ledId, isLedOn), ZToolCmdType.UtilLedControlRsp);

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Trying to set network coordinator led {LedId} to {IsLedOn}. Response status {Status}.", ledId, isLedOn, response.Status);

        return response.Status == ZToolPacketStatus.Success;
    }

    public bool StartupNetwork(ushort startupDelay)
    {
        var response = _packetReceiverTransmitterService.SendAndWaitForResponse<IZdoStartupFromAppResponse>(new ZdoStartupFromAppRequest(startupDelay), ZToolCmdType.ZdoStartupFromAppRsp);

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Trying to start up network. Response status {Status}.", response.Status);

        return response.Status is ZToolZdoStartupFromAppStatus.NewNetworkState or ZToolZdoStartupFromAppStatus.RestoredNwkState;
    }

    #region NetworkEndpoints
    public List<NetworkEndpoint> GetSupportedEndpoints()
    {
        const byte GreenPowerClusterEp = 242;

        ushort defAppProfId = 0x0104;
        ushort defDeviceId = 0x0005;
        byte defAppDevVer = 0;
        byte defAppNumberInClusters = 0;
        var defAppInClusterList = Array.Empty<ushort>();
        byte defAppNumberOutClusters = 0;
        var defAppOutClusterList = Array.Empty<ushort>();
        var defLatencyReq = AfRegisterLatency.NoLatency;

        return
        [
            new(1, defAppProfId, defDeviceId, defAppDevVer, defLatencyReq, defAppNumberInClusters, defAppInClusterList, defAppNumberOutClusters, defAppOutClusterList),
            new(2, 0x0101, defDeviceId, defAppDevVer, defLatencyReq, defAppNumberInClusters, defAppInClusterList, defAppNumberOutClusters, defAppOutClusterList),
            new(3, defAppProfId, defDeviceId, defAppDevVer, defLatencyReq, defAppNumberInClusters, defAppInClusterList, defAppNumberOutClusters, defAppOutClusterList),
            new(4, 0x0107, defDeviceId, defAppDevVer, defLatencyReq, defAppNumberInClusters, defAppInClusterList, defAppNumberOutClusters, defAppOutClusterList),
            new(5, 0x0108, defDeviceId, defAppDevVer, defLatencyReq, defAppNumberInClusters, defAppInClusterList, defAppNumberOutClusters, defAppOutClusterList),
            new(6, 0x0109, defDeviceId, defAppDevVer, defLatencyReq, defAppNumberInClusters, defAppInClusterList, defAppNumberOutClusters, defAppOutClusterList),
            new(8, defAppProfId, defDeviceId, defAppDevVer, defLatencyReq, defAppNumberInClusters, defAppInClusterList, defAppNumberOutClusters, defAppOutClusterList),
            new(10, defAppProfId, defDeviceId, defAppDevVer, defLatencyReq, defAppNumberInClusters, defAppInClusterList, defAppNumberOutClusters, defAppOutClusterList),
            // Cluster id is ushort, I'm converting it to byte
            new(11, defAppProfId, 0x0400, defAppDevVer, defLatencyReq, 1, [0x0501] , 2, [0x0500, 0x0502]),
            new(12, 0xc05e, defDeviceId, defAppDevVer, defLatencyReq, defAppNumberInClusters, defAppInClusterList, defAppNumberOutClusters, defAppOutClusterList),
            new(13, defAppProfId, defDeviceId, defAppDevVer, defLatencyReq, 1, [0x0019] , defAppNumberOutClusters, defAppOutClusterList),
            new(47, defAppProfId, defDeviceId, defAppDevVer, defLatencyReq, defAppNumberInClusters, defAppInClusterList, defAppNumberOutClusters, defAppOutClusterList),
            new(110, defAppProfId, defDeviceId, defAppDevVer, defLatencyReq, defAppNumberInClusters, defAppInClusterList, defAppNumberOutClusters, defAppOutClusterList),
            new(GreenPowerClusterEp, 0xa1e0, defDeviceId, defAppDevVer, defLatencyReq, defAppNumberInClusters, defAppInClusterList, defAppNumberOutClusters, defAppOutClusterList)
        ];
    }

    public void RegisterEndpoints(IEnumerable<NetworkEndpoint> endpoints)
    {
        foreach (var endpoint in endpoints)
        {
            var request = new AfRegisterRequest(endpoint.EndpointId,
                endpoint.AppProfId,
                endpoint.AppDeviceId,
                endpoint.AppDevVersion,
                endpoint.Latency,
                endpoint.AppNumInClusters,
                endpoint.AppInClusterList,
                endpoint.AppNumOutClusters,
                endpoint.AppOutClusterList);

            IAfRegisterResponse? response = null;

            try
            {
                response = _packetReceiverTransmitterService.SendAndWaitForResponse<IAfRegisterResponse>(request, ZToolCmdType.AfRegisterRsp);
            }
            catch (Exception e)
            {
                throw new NetworkException($"Cannot register endpoint {endpoint.EndpointId}", e);
            }

            if (response.Status != ZToolPacketStatus.Success)
                throw new NetworkException($"Cannot register endpoint {endpoint.EndpointId}");

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Ednpoint {EndpointId} successfully registered.", endpoint.EndpointId);
        }
    }

    public byte[] GetActiveEndpointIds()
    {
        var activeEndpointResponse = _packetReceiverTransmitterService.SendAndWaitForResponse<IZdoActiveEpCallback>(new ZdoActiveEpRequest(0, 0), ZToolCmdType.ZdoActiveEpClbk);

        return activeEndpointResponse.Status != ZToolPacketStatus.Success
            ? throw new NetworkException("Active endpoint request failed.")
            : activeEndpointResponse.ActiveEps;
    }

    public void ValidateRegisteredEndpoints(IEnumerable<byte> endpointIds)
    {
        foreach (var endpointId in endpointIds)
        {
            IZdoSimpleDescCallback? response = null;

            try
            {
                response = _packetReceiverTransmitterService.SendAndWaitForResponse<IZdoSimpleDescCallback>(new ZdoSimpleDescRequest(0, 0, endpointId), ZToolCmdType.ZdoSimpleDescClbk);
            }
            catch(Exception e)
            {
                throw new NetworkException($"Endpoint validation failed {endpointId}.", e);
            }

            if (response.Status != ZToolPacketStatus.Success)
                throw new NetworkException($"Endpoint validation failed {endpointId}.");
        }
    }
    #endregion NetworkEndpoints
}
