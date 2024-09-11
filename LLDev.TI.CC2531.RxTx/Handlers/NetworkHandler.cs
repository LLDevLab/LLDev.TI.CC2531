using LLDev.TI.CC2531.RxTx.Devices;
using LLDev.TI.CC2531.RxTx.Models;
using LLDev.TI.CC2531.RxTx.Services;
using Microsoft.Extensions.Logging;

namespace LLDev.TI.CC2531.RxTx.Handlers;

public interface INetworkHandler
{
    event DeviceAnnouncedHandler? DeviceAnnouncedAsync;
    event EndDeviceMessageReceivedHandler? DeviceMessageReceivedAsync;

    DeviceInfo? NetworkCoordinatorInfo { get; }

    void StartZigBeeNetwork();
    void PermitNetworkJoin(bool isJoinPermitted);
}

internal sealed class NetworkHandler : INetworkHandler
{
    private readonly INetworkCoordinatorService _networkCoordinatorService;
    private readonly INetworkDevice _networkDevice;
    private readonly ILogger<NetworkHandler> _logger;

    public event DeviceAnnouncedHandler? DeviceAnnouncedAsync
    {
        add => _networkDevice.DeviceAnnouncedAsync += value;
        remove => _networkDevice.DeviceAnnouncedAsync -= value;
    }

    public event EndDeviceMessageReceivedHandler? DeviceMessageReceivedAsync
    {
        add => _networkDevice.DeviceMessageReceivedAsync += value;
        remove => _networkDevice.DeviceMessageReceivedAsync -= value;
    }

    public DeviceInfo? NetworkCoordinatorInfo { get; private set; } = null;

    internal NetworkHandler(INetworkCoordinatorService networkCoordinatorService,
        INetworkDevice networkDevice,
        ILogger<NetworkHandler> logger)
    {
        _networkCoordinatorService = networkCoordinatorService;
        _networkDevice = networkDevice;
        _logger = logger;
    }

    public void StartZigBeeNetwork()
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Starting ZigBee network.");

        NetworkCoordinatorInfo = _networkCoordinatorService.StartupCoordinator();
        _networkCoordinatorService.RegisterNetworkEndpoints();

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("ZigBee network successfully started.");
    }

    public void PermitNetworkJoin(bool isJoinPermitted)
    {
        _networkCoordinatorService.SetStatusLedMode(isJoinPermitted);
        _networkCoordinatorService.PermitNetworkJoin(isJoinPermitted);
    }
}
