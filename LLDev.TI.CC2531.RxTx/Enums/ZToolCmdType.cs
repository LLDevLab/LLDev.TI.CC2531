namespace LLDev.TI.CC2531.RxTx.Enums;
internal enum ZToolCmdType : ushort
{
    Unknown = 0x0000,

    /// <summary>
    /// This command issues PING requests to verify if a device is active and check the capability of the device.
    /// </summary>
    SysPingReq = 0x2101,

    /// <summary>
    /// This command is used to request for the device’s version string (request).
    /// </summary>
    SysVersionReq = 0x2102,

    /// <summary>
    /// This command is used to get the extended address of the device.
    /// </summary>
    SysGetExtAddrReq = 0x2104,

    /// <summary>
    /// This command is used by the tester to read a single memory item from the target non-volatile memory.
    /// </summary>
    SysOsalNvReadReq = 0x2108,

    /// <summary>
    /// This command is used by the tester to get the length of an item in non-volatile memory.
    /// </summary>
    SysOsalNvLengthReq = 0x2113,

    /// <summary>
    /// This command enables the tester to register an application’s endpoint description.
    /// </summary>
    AfRegisterReq = 0x2400,

    /// <summary>
    /// This command is used by tester to build and send a data request message
    /// </summary>
    AfDataReq = 0x2401,

    /// <summary>
    /// This command will request a device’s IEEE 64-bit address
    /// </summary>
    ZdoIeeeAddrReq = 0x2501,

    /// <summary>
    /// This command is generated to inquire about the Node Descriptor information of the destination device
    /// </summary>
    ZdoNodeDescReq = 0x2502,

    /// <summary>
    /// This command is generated to inquire as to the Simple Descriptor of the destination device’s Endpoint.
    /// </summary>
    ZdoSimpleDescReq = 0x2504,

    /// <summary>
    /// This command is generated to request a list of active endpoint from the destination device.
    /// </summary>
    ZdoActiveEpReq = 0x2505,

    /// <summary>
    /// This command is used to initiate a network discovery (active scan).
    /// </summary>
    ZdoNwkDiscoveryReq = 0x2526,

    /// <summary>
    /// This command registers for a ZDO callback (request)
    /// </summary>
    ZdoMsgCbRegisterReq = 0x253e,

    /// <summary>
    /// This command starts the device in the network (request).
    /// </summary>
    ZdoStartupFromAppReq = 0x2540,

    /// <summary>
    /// This command handles the ZDO route discovery extension message
    /// </summary>
    ZdoExtRouteDicsReq = 0x2545,

    /// <summary>
    /// This command handles the ZDO extension find group message
    /// </summary>
    ZdoExtFindGroupReq = 0x254a,

    /// <summary>
    /// This command handles the ZDO extension network message
    /// </summary>
    ZdoExtNwkInfoReq = 0x2550,

    /// <summary>
    /// Reads a configuration property from nonvolatile memory (request).
    /// </summary>
    ZbReadConfigurationReq = 0x2604,

    /// <summary>
    /// Writes a configuration property to nonvolatile memory (request).
    /// </summary>
    ZbWriteConfigurationReq = 0x2605,

    /// <summary>
    /// This command retrieves a Device Information Property (request).
    /// </summary>
    ZbGetDeviceInfoReq = 0x2606,

    /// <summary>
    /// This command is sent by the tester to retrieve the device info.
    /// </summary>
    UtilGetDeviceInfoReq = 0x2700,

    /// <summary>
    /// This command is used by the tester to control the LEDs on the board (request).
    /// </summary>
    UtilLedControlReq = 0x270a,

    /// <summary>
    /// This command is sent by the tester to the target to reset it
    /// </summary>
    SysResetReq = 0x4100,

    /// <summary>
    /// This callback is sent by the device to indicate that a reset has occurred.
    /// </summary>
    SysResetIndClbk = 0x4180,

    /// <summary>
    /// This callback message is in response to incoming data to any of the registered endpoints on this device.
    /// </summary>
    AfIncomingMsgClbk = 0x4481,

    /// <summary>
    /// This command is issued by the tester to return the results from a ZdoNwkAddrReq.
    /// </summary>
    ZdoNwkAddrClbk = 0x4580,

    /// <summary>
    /// This callback message is in response to the ZDO IEEE Address Request.
    /// </summary>
    ZdoIeeeAddrClbk = 0x4581,

    /// <summary>
    /// This callback message is in response to the ZDO Node Descriptor Request.
    /// </summary>
    ZdoNodeDescClbk = 0x4582,

    /// <summary>
    /// This callback message is in response to the ZDO Simple Descriptor Request
    /// </summary>
    ZdoSimpleDescClbk = 0x4584,

    /// <summary>
    /// This callback message is in response to the ZDO Active Endpoint Request.
    /// </summary>
    ZdoActiveEpClbk = 0x4585,

    /// <summary>
    /// This callback message indicates the ZDO state change.
    /// </summary>
    ZdoStateChangedIndClbk = 0x45c0,

    /// <summary>
    /// This callback indicates the ZDO End Device Announce.
    /// </summary>
    ZdoEndDeviceAnnceIndClbk = 0x45c1,

    /// <summary>
    /// This message is a ZDO callback for a Cluster Id that the host requested to receive with a ZdoMsgCbRegister request.
    /// </summary>
    ZdoMsgCbIncomingClbk = 0x45ff,

    /// <summary>
    /// Response for SysPingReq
    /// </summary>
    SysPingRsp = 0x6101,

    /// <summary>
    /// This command is used to request for the device’s version string (response).
    /// </summary>
    SysVersionRsp = 0x6102,

    /// <summary>
    /// Response for SysGetExtAddrReq message
    /// </summary>
    SysGetExtAddrRsp = 0x6104,

    /// <summary>
    /// Response for SysOsalNvReadReq message
    /// </summary>
    SysOsalNvReadRsp = 0x6108,

    /// <summary>
    /// Response for SysOsalNvLengthReq message
    /// </summary>
    SysOsalNvLengthRsp = 0x6113,

    /// <summary>
    /// Response for AfRegisterReq message
    /// </summary>
    AfRegisterRsp = 0x6400,

    /// <summary>
    /// This command is used by the tester to build and send a message through AF layer.
    /// </summary>
    AfDataRsp = 0x6401,

    /// <summary>
    /// Response for ZdoIeeeAddrReq request
    /// </summary>
    ZdoIeeeAddrRsp = 0x6501,

    /// <summary>
    /// Response for ZdoNodeDescReq request
    /// </summary>
    ZdoNodeDescRsp = 0x6502,

    /// <summary>
    /// Response for ZdoSimpleDescReq message
    /// </summary>
    ZdoSimpleDescRsp = 0x6504,

    /// <summary>
    /// Response for ZdoActiveEpReq
    /// </summary>
    ZdoActiveEpRsp = 0x6505,

    /// <summary>
    /// Response for ZdoNwkDiscoveryReq
    /// </summary>
    ZdoNwkDiscoveryRsp = 0x6526,

    /// <summary>
    /// This command registers for a ZDO callback (response).
    /// </summary>
    ZdoMsgCbRegisterRsp = 0x653e,

    /// <summary>
    /// This command starts the device in the network (response).
    /// </summary>
    ZdoStartupFromAppRsp = 0x6540,

    /// <summary>
    /// Response for ZdoExtRouteDiscReq request
    /// </summary>
    ZdoExtRouteDiscRsp = 0x6545,

    /// <summary>
    /// Response for ZdoExtFindGroupReq message
    /// </summary>
    ZdoExtFindGroupRsp = 0x654a,

    /// <summary>
    /// Response for ZdoExtNwkInfoReq message
    /// </summary>
    ZdoExtNwkInfoRsp = 0x6550,

    /// <summary>
    /// This command is used to get a configuration property from nonvolatile memory (response).
    /// </summary>
    ZbReadConfigurationRsp = 0x6604,

    /// <summary>
    /// This command is used to write a Configuration Property to nonvolatile memory (response).
    /// </summary>
    ZbWriteConfigurationRsp = 0x6605,

    /// <summary>
    /// This command retrieves a Device Information Property (response).
    /// </summary>
    ZbGetDeviceInfoRsp = 0x6606,

    /// <summary>
    /// Response for UtilGetDeviceInfoReq request
    /// </summary>
    UtilGetDeviceInfoRsp = 0x6700,

    /// <summary>
    /// This command is used by the tester to control the LEDs on the board (response).
    /// </summary>
    UtilLedControlRsp = 0x670a
}
