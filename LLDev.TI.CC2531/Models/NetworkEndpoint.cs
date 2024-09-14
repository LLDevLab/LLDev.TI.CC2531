using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Models;
internal record NetworkEndpoint(byte EndpointId,
    ushort AppProfId,
    ushort AppDeviceId,
    byte AppDevVersion,
    AfRegisterLatency Latency,
    byte AppNumInClusters,
    ushort[] AppInClusterList,
    byte AppNumOutClusters,
    ushort[] AppOutClusterList);
