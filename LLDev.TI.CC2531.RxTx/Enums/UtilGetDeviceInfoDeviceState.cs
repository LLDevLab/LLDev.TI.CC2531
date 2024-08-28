namespace LLDev.TI.CC2531.RxTx.Enums;
internal enum UtilGetDeviceInfoDeviceState : byte
{
    /// <summary>
    /// Initialized - not started automatically
    /// </summary>
    InitNotStarted = 0x00,

    /// <summary>
    /// Initialized - not connected to anything
    /// </summary>
    InitNotConnected = 0x01,

    DiscoveringPansToJoin = 0x02,
    JoiningPan = 0x03,

    /// <summary>
    /// Rejoining a PAN, only for end devices
    /// </summary>
    RejoiningPan = 0x04,

    /// <summary>
    /// Joined but not yet authenticated by trust center
    /// </summary>
    JoinedNotAuth = 0x05,

    /// <summary>
    /// Started as device after authentication
    /// </summary>
    StartedAsDeviceAuth = 0x06,

    /// <summary>
    /// Device joined, authenticated and is a router
    /// </summary>
    StartedAsRouterAuth = 0x07,

    /// <summary>
    /// Starting as ZigBee Coordinator
    /// </summary>
    StartingAsCoordinator = 0x08,

    /// <summary>
    /// Started as ZigBee Coordinator
    /// </summary>
    StartedAsCoordinator = 0x09,

    /// <summary>
    /// Device has lost information about its parent
    /// </summary>
    Orphan = 0x0a
}