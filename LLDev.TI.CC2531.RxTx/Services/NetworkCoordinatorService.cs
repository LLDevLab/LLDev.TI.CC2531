using LLDev.TI.CC2531.RxTx.Devices;
using LLDev.TI.CC2531.RxTx.Exceptions;
using LLDev.TI.CC2531.RxTx.Models;
using Microsoft.Extensions.Logging;

namespace LLDev.TI.CC2531.RxTx.Services;

internal interface INetworkCoordinatorService
{
    DeviceInfo StartupCoordinator();
    void SetStatusLedMode(bool value);
    void RegisterNetworkEndpoints();
    bool PermitNetworkJoin(bool isPermited);
}

internal sealed class NetworkCoordinatorService(INetworkCoordinator networkCoordinator,
    ITransactionService transactionService,
    ILogger<NetworkCoordinatorService> logger) : INetworkCoordinatorService
{
    private readonly INetworkCoordinator _networkCoordinator = networkCoordinator;
    private readonly ITransactionService _transactionService = transactionService;
    private readonly ILogger<NetworkCoordinatorService> _logger = logger;

    public DeviceInfo StartupCoordinator()
    {
        const ushort StartupDelay = 100;

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Starting up network coordinator.");

        _networkCoordinator.Initialize();

        _networkCoordinator.PingCoordinatorOrThrow();
        _networkCoordinator.ResetCoordinator();

        if (!_networkCoordinator.StartupNetwork(StartupDelay))
            throw new NetworkException("Cannot start up the network.");

        var deviceInfo = _networkCoordinator.GetCoordinatorInfo();

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Network coordinator succesfully started up.");

        return deviceInfo;
    }

    public void SetStatusLedMode(bool value)
    {
        const int StatusLedId = 1;

        if (!_networkCoordinator.SetCoordinatorLedMode(StatusLedId, value) && _logger.IsEnabled(LogLevel.Warning))
            _logger.LogWarning("Failed to set status LED");
    }

    public void RegisterNetworkEndpoints()
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Endpoints registration started.");

        var activeEndpointIds = _networkCoordinator.GetActiveEndpointIds();

        var endpointsToActivate = _networkCoordinator.GetSupportedEndpoints();

        _networkCoordinator.RegisterEndpoints(endpointsToActivate.Where(x => !activeEndpointIds.Contains(x.EndpointId)));

        activeEndpointIds = _networkCoordinator.GetActiveEndpointIds();

        if (!endpointsToActivate.All(x => activeEndpointIds.Any(y => y == x.EndpointId)))
            throw new NetworkException("Failed to register all endpoints.");

        _networkCoordinator.ValidateRegisteredEndpoints(endpointsToActivate.Select(ep => ep.EndpointId));

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Endpoints registration finished successfully.");
    }

    public bool PermitNetworkJoin(bool isPermited)
    {
        var transactionId = _transactionService.GetNextTransactionId();
        return _networkCoordinator.PermitNetworkJoin(transactionId, isPermited);
    }
}
