# LLDev.TI.CC2531

[![NuGet](https://img.shields.io/nuget/v/LLDev.TI.CC2531.svg)](https://www.nuget.org/packages/LLDev.TI.CC2531)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Unit tests](https://github.com/LLDevLab/LLDev.TI.CC2531/actions/workflows/unit-tests.yml/badge.svg)](https://github.com/LLDevLab/LLDev.TI.CC2531/actions/workflows/unit-tests.yml)

A .NET library for communicating with ZigBee devices using the **Texas Instruments CC2531** USB dongle as a ZigBee network coordinator (ZNP — ZigBee Network Processor).

## Features

- Acts as a ZigBee **network coordinator** — creates and manages the network
- Starts the network and controls **permit join** (with status LED feedback)
- Receives **device announce** events when a new device joins the network
- Receives **incoming messages** from end devices (cluster ID + raw payload)
- Configurable serial port and response timeout via `IConfiguration`
- Built on `Microsoft.Extensions.DependencyInjection` — registers as a singleton
- Targets **.NET 8 / 9 / 10 / 11**

> **Note:** This package currently supports **receiving** messages from ZigBee devices. Sending messages to devices is not yet implemented.

## Prerequisites — flashing the CC2531

The CC2531 must be flashed with **ZStack firmware** (ZNP — ZigBee Network Processor). Here are some guides:

- [Flashing the CC2531 (zigbee2mqtt.io)](https://www.zigbee2mqtt.io/guide/adapters/flashing/flashing_the_cc2531.html)
- [Alternative flashing methods (zigbee2mqtt.io)](https://www.zigbee2mqtt.io/guide/adapters/flashing/alternative_flashing_methods.html)

Pre-built ZStack firmware can be found in the [Koenkk/Z-Stack-firmware](https://github.com/Koenkk/Z-Stack-firmware) repository.

## Installation

Install the package from NuGet:

```bash
dotnet add package LLDev.TI.CC2531
```

Or via the Package Manager Console:

```powershell
Install-Package LLDev.TI.CC2531
```

## Configuration

Add the serial port settings to your `appsettings.json` (or any `IConfiguration` section):

```json
"ZigBee": {
  "PortName": "COM3",
  "WaitResponseTimeoutMs": 2000
}
```

| Setting | Required | Default | Description |
|---|---|---|---|
| `PortName` | ✅ Yes | — | Serial port name. `COM3` on Windows, `/dev/ttyACM0` on Linux. |
| `WaitResponseTimeoutMs` | ❌ No | `2000` | How long (ms) to wait for a response from the CC2531 before throwing. Increase this if you see `"Cannot receive response within specified duration"` exceptions. |

**Linux example:**

```json
"ZigBee": {
  "PortName": "/dev/ttyACM0"
}
```

> On Linux you may need to add your user to the `dialout` group so the process can access the serial device:
> ```bash
> sudo usermod -a -G dialout $USER
> ```

## Usage

### 1. Register services

```csharp
using LLDev.TI.CC2531.Extensions;

// In Program.cs / Startup.cs
builder.Services.AddZigBeeServices(builder.Configuration.GetSection("ZigBee"));
```

This registers `INetworkHandler` as a **singleton** in the DI container.

### 2. Resolve the handler and subscribe to events

```csharp
using LLDev.TI.CC2531.Enums;
using LLDev.TI.CC2531.Handlers;
using LLDev.TI.CC2531.Models;

public class ZigBeeListener(INetworkHandler networkHandler)
{
    public void Start()
    {
        networkHandler.DeviceAnnouncedAsync += OnDeviceAnnounced;
        networkHandler.DeviceMessageReceivedAsync += OnDeviceMessageReceived;

        networkHandler.StartZigBeeNetwork();
        networkHandler.PermitNetworkJoin(true); // allow devices to join
    }

    private Task OnDeviceAnnounced(DeviceAnnounceInfo info)
    {
        Console.WriteLine(
            $"Device joined: IEEE={info.IeeeAddr:X16}, NWK={info.NwkAddr:X4}, " +
            $"MainPowered={info.IsMainPowered}, Secure={info.IsSecure}");
        return Task.CompletedTask;
    }

    private Task OnDeviceMessageReceived(ushort nwkAddr, ZigBeeClusterId clusterId, byte[] message)
    {
        Console.WriteLine(
            $"Message from 0x{nwkAddr:X4} on cluster {clusterId} (0x{(ushort)clusterId:X4}): " +
            $"{Convert.ToHexString(message)}");
        return Task.CompletedTask;
    }
}
```

### 3. Start the network

```csharp
networkHandler.StartZigBeeNetwork();       // creates / restores the ZigBee network
networkHandler.PermitNetworkJoin(true);    // open the network for new devices (LED turns on)
// …
networkHandler.PermitNetworkJoin(false);   // close the network (LED turns off)
```

After `StartZigBeeNetwork()` succeeds, the coordinator's address info is available via:

```csharp
DeviceInfo? info = networkHandler.NetworkCoordinatorInfo;
// info.IeeeAddr — 64-bit IEEE address
// info.NwkAddr  — 16-bit network address (typically 0x0000 for the coordinator)
```

## API reference

### `INetworkHandler`

| Member | Type | Description |
|---|---|---|
| `DeviceAnnouncedAsync` | `event DeviceAnnouncedHandler?` | Raised when a device announces itself on the network. |
| `DeviceMessageReceivedAsync` | `event EndDeviceMessageReceivedHandler?` | Raised when a message is received from an end device. |
| `NetworkCoordinatorInfo` | `DeviceInfo?` | Coordinator info after `StartZigBeeNetwork()`. `null` before. |
| `StartZigBeeNetwork()` | `void` | Initializes the coordinator, starts the network, registers endpoints. |
| `PermitNetworkJoin(bool)` | `void` | Opens/closes the network for joining devices. Also toggles the status LED. |

### Event delegates

```csharp
public delegate Task DeviceAnnouncedHandler(DeviceAnnounceInfo deviceAnnounceInfo);
public delegate Task EndDeviceMessageReceivedHandler(ushort nwkAddr, ZigBeeClusterId clusterId, byte[] message);
```

### `DeviceAnnounceInfo`

| Property | Type | Description |
|---|---|---|
| `IeeeAddr` | `ulong` | 64-bit IEEE (MAC) address of the device. |
| `NwkAddr` | `ushort` | 16-bit network address assigned to the device. |
| `SrcAddr` | `ushort` | Source address of the announce. |
| `IsMainPowered` | `bool` | Device is mains-powered. |
| `IsReceiverOnWhenIdle` | `bool` | Receiver is on when idle. |
| `IsSecure` | `bool` | Device joined with security. |

### `ZigBeeClusterId` (common clusters)

| Cluster | Value |
|---|---|
| `PermitJoin` | `0x0036` |
| `TemperatureMeasurement` | `0x0402` |
| `PressureMeasurement` | `0x0403` |
| `RelativeHumidityMeasurement` | `0x0405` |
| `IndustrialAlarmSystemZone` | `0x0500` |

## Troubleshooting

| Problem | Solution |
|---|---|
| `Cannot receive response within specified duration X ms` | Increase `WaitResponseTimeoutMs` in configuration. Check that the correct `PortName` is set and the CC2531 is plugged in. |
| `Cannot ping network coordinator` | The CC2531 may not be flashed with ZStack firmware, or the wrong COM port is selected. Verify the firmware and port. |
| No events firing | Ensure `StartZigBeeNetwork()` completed successfully and `PermitNetworkJoin(true)` was called. Check that your event handlers return a `Task`. |
| Permission denied on `/dev/ttyACM0` (Linux) | Add your user to the `dialout` group: `sudo usermod -a -G dialout $USER`, then log out and back in. |

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on [GitHub](https://github.com/LLDevLab/LLDev.TI.CC2531).

## License

This project is licensed under the [MIT License](LICENSE).
