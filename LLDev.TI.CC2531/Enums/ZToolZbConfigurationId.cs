namespace LLDev.TI.CC2531.Enums;
internal enum ZToolZbConfigurationId : byte
{
    NvStartupOption = 0x03,
    NvPollRate = 0x24,
    NvQueuedPollRate = 0x25,
    NvResponsePollRate = 0x26,
    NvPollFailureRatries = 0x29,
    NvIndirectMsgTimeout = 0x2b,
    NvRouteExpiryTime = 0x2c,
    NvExtPanId = 0x2d,
    NvBCastRetries = 0x2e,
    NvPassiveAckTimeout = 0x2f,
    NvBCastDeliveryTime = 0x30,
    NvApsFrameRetries = 0x43,
    NvApsAckWaitDuration = 0x44,
    NvBindingTime = 0x46,
    NvPreCfgKey = 0x62,
    NvPreCfgKeysEnable = 0x63,
    NvSecurityMode = 0x64,
    NvUseDefaultTc = 0x6d,
    NvUserDesc = 0x81,
    NvPanId = 0x83,
    NvChannelList = 0x84,
    NvLogicalType = 0x87,
    NvDirectCb = 0x8f
}
