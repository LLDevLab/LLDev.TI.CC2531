# LLDev.TI.CC2531
This package allow to communicate with ZigBee devices using Texas Instruments CC2531 as a ZigBee network controller in .NET projects.

## CC2531 Setup
CC2531 should have ZStack firmware. Internet have many instructions how to flash firmware to the device. Here are some of them:
- https://www.zigbee2mqtt.io/guide/adapters/flashing/flashing_the_cc2531.html
- https://www.zigbee2mqtt.io/guide/adapters/flashing/alternative_flashing_methods.html

ZStack firmware could be found [here](https://github.com/Koenkk/Z-Stack-firmware).

## Using the package
To use the package, first of all it should be added to .NET dependency injection system. To to it, you need to define port that package should use. It can be done by adding port to appsettings.json file:

```
"MyJsonSection": {
  "PortName": "COM0" // in windows,
  //"PortName": "/dev/ttyACM0" // in linux
},
```

If you are catching "Cannot receive response within specified duration X ms" exception, you could also increase wait duration from appsettings:

```
"MyJsonSection": {
  "PortName": "..."
  "WaitResponseTimeoutMs": 9999
},
```

*Default wait duration is 2 seconds.*

Then you need to call AddZigBeeServices method:

```
using LLDev.TI.CC2531.Extensions;

...
IConfiguration configuration;
IServiceCollection services;
services.AddZigBeeServices(configuration.GetSection("MyJsonSection"));
```

It will add LLDev.TI.CC2531.Handlers.INetworkHandler object to dependency injection system as a singleton.

When it is done, you need to start the network and allow devices to join it. It could be done so:

```
using LLDev.TI.CC2531.Handlers;

LLDev.TI.CC2531.Handlers.INetworkHandler networkHandler;

networkHandler.StartZigBeeNetwork();
networkHandler.PermitNetworkJoin(true);
```

LLDev.TI.CC2531.Handlers.INetworkHandler have 2 events:

```
event DeviceAnnouncedHandler? DeviceAnnouncedAsync;
event EndDeviceMessageReceivedHandler? DeviceMessageReceivedAsync;
```

*DeviceAnnouncedAsync* event will be called when device is connecting to the network.

*DeviceMessageReceivedAsync* event will be called when device message received.

**NB! Currently this package is allowing to receive device messages, and do not allow to send messages to ZigBee device.**
