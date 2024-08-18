namespace LLDev.TI.CC2531.RxTx.Enums;

/// <summary>
/// State for ZdoStateChangedInd packet
/// </summary>
public enum ZToolZdoState : byte
{
    /// <summary>
    /// Initialized - not started automatically
    /// </summary>
    Hold = 0x00,

    /// <summary>
    /// Initialized - not connected to anything
    /// </summary>
    Init = 0x01,

    /// <summary>
    /// Discovering PAN's to join
    /// </summary>
    NwkDisc = 0x02,

    /// <summary>
    /// Joining a PAN
    /// </summary>
    NwkJoining = 0x03,

    /// <summary>
    /// Rejoining a PAN, only for end-devices
    /// </summary>
    NwkRejoining = 0x04,

    /// <summary>
    /// Joined but not yet authenticated by trust center
    /// </summary>
    EndDeviceUnauth = 0x05,

    /// <summary>
    /// Started as device after authentication
    /// </summary>
    EndDevice = 0x06,

    /// <summary>
    /// Device joined, authenticated and is a router
    /// </summary>
    Router = 0x07,

    /// <summary>
    /// Started as ZigBee Coordinator
    /// </summary>
    CoordStarting = 0x08,

    /// <summary>
    /// Started as ZigBee Coordinator
    /// </summary>
    ZbCoord = 0x09,

    /// <summary>
    /// Device has lost information about its parent
    /// </summary>
    NwkOrphan = 0x0A
}
