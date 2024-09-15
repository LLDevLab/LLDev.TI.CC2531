using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Models;
internal record NetworkEndpoint(byte EndpointId,
    ushort AppProfId,
    ushort AppDeviceId,
    byte AppDevVersion,
    AfRegisterLatency Latency,
    byte AppNumInClusters,
    ushort[] AppInClusterList,
    byte AppNumOutClusters,
    ushort[] AppOutClusterList);
