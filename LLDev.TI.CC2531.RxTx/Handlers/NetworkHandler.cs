using LLDev.TI.CC2531.RxTx.Devices;
using LLDev.TI.CC2531.RxTx.Models;
using LLDev.TI.CC2531.RxTx.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLDev.TI.CC2531.RxTx.Handlers;

public interface INetworkHandler
{
    event DeviceAnnouncedHandler? DeviceAnnouncedAsync;
    event EndDeviceMessageReceivedHandler? DeviceMessageReceivedAsync;

    DeviceInfo? NetworkCoordinatorInfo { get; }

    void StartZigBeeNetwork();
}

public sealed class NetworkHandler(INetworkCoordinatorService networkCoordinatorService,
        INetworkDevice networkDevice,
        ILogger<NetworkHandler> logger) : INetworkHandler
{
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

    private readonly INetworkCoordinatorService _networkCoordinatorService = networkCoordinatorService;
    private readonly INetworkDevice _networkDevice = networkDevice;
    private readonly ILogger<NetworkHandler> _logger = logger;

    public void StartZigBeeNetwork()
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Starting ZigBee network.");
    }
}
