using LLDev.TI.CC2531.Devices;
using LLDev.TI.CC2531.Models;
using LLDev.TI.CC2531.Services;
using Microsoft.Extensions.Logging;

namespace LLDev.TI.CC2531.Handlers;

public interface INetworkHandler
{
    event DeviceAnnouncedHandler? DeviceAnnouncedAsync;
    event EndDeviceMessageReceivedHandler? DeviceMessageReceivedAsync;

    DeviceInfo? NetworkCoordinatorInfo { get; }

    void StartZigBeeNetwork();
    void PermitNetworkJoin(bool isJoinPermitted);
}

internal sealed class NetworkHandler(INetworkCoordinatorService networkCoordinatorService,
    INetworkDevice networkDevice,
    ILogger<NetworkHandler> logger) : INetworkHandler
{
    private readonly INetworkCoordinatorService _networkCoordinatorService = networkCoordinatorService;
    private readonly INetworkDevice _networkDevice = networkDevice;
    private readonly ILogger<NetworkHandler> _logger = logger;

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
