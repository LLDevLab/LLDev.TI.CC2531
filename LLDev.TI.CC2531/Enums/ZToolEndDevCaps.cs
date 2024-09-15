namespace LLDev.TI.CC2531.Enums;

/// <summary>
/// End device capability flags
/// </summary>
[Flags]
internal enum ZToolEndDevCaps : byte
{
    /// <summary>
    /// Alternative PAN coordinator
    /// </summary>
    AltPanCoord = 0x01,
    /// <summary>
    /// Device type: 1- ZigBee Router; 0 – End Device
    /// </summary>
    DeviceType = 0x02,
    /// <summary>
    /// Power Source: 1 Main powered
    /// </summary>
    PowerSource = 0x04,
    /// <summary>
    /// Receiver on when Idle
    /// </summary>
    ReceiverOnWhenIdle = 0x08,
    Reserved1 = 0x10,
    Reserved2 = 0x20,
    Security = 0x40,
    Reserved3 = 0x80
}
