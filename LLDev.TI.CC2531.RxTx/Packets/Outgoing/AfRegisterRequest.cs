using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Packets.Outgoing;
public sealed class AfRegisterRequest : OutgoingPacket, IOutgoingPacket
{
    /// <summary>
    /// Specifies the endpoint of the device
    /// </summary>
    public byte Endpoint { get; }

    /// <summary>
    /// Specifies the profile Id of the application
    /// </summary>
    public ushort AppProfId { get; }

    /// <summary>
    /// Specifies the device description Id for this endpoint
    /// </summary>
    public ushort AppDeviceId { get; }

    /// <summary>
    /// Specifies the device version number
    /// </summary>
    public byte AppDevVersion { get; }

    public AfRegisterLatency LatencyReq { get; }

    /// <summary>
    /// The number of Input cluster Id’s following in the AppInClusterList
    /// </summary>
    public byte AppNumInClusters { get; }

    /// <summary>
    /// Specifies the list of Input Cluster Id’s
    /// </summary>
    public ushort[] AppInClusterList { get; }

    /// <summary>
    /// Specifies the number of Output cluster Id’s following in the AppOutClusterList
    /// </summary>
    public byte AppNumOutClusters { get; }

    /// <summary>
    /// Specifies the list of Output Cluster Id’s
    /// </summary>
    public ushort[] AppOutClusterList { get; }

    protected override byte[] Data { get; }

    public AfRegisterRequest(byte endpoint,
        ushort appProfId,
        ushort appDeviceId,
        byte appDevVersion,
        AfRegisterLatency latencyReq,
        byte appNumInClusters,
        ushort[] appInClusterList,
        byte appNumOutClusters,
        ushort[] appOutClusterList) : base(ZToolCmdType.AfRegisterReq, (byte)(9 + (appInClusterList.Length * 2) + (appOutClusterList.Length * 2)))
    {
        Endpoint = endpoint;
        AppProfId = appProfId;
        AppDeviceId = appDeviceId;
        AppDevVersion = appDevVersion;
        LatencyReq = latencyReq;
        AppNumInClusters = appNumInClusters;
        AppInClusterList = appInClusterList;
        AppNumOutClusters = appNumOutClusters;
        AppOutClusterList = appOutClusterList;

        Data = GetData();
    }

    private byte[] GetData()
    {
        var dataList = new List<byte>
        {
            Endpoint,
            GetLsb(AppProfId),
            GetMsb(AppProfId),
            GetLsb(AppDeviceId),
            GetMsb(AppDeviceId),
            AppDevVersion,
            (byte)LatencyReq,
            AppNumInClusters
        };
        dataList.AddRange(GetBytes(AppInClusterList));
        dataList.Add(AppNumOutClusters);
        dataList.AddRange(GetBytes(AppOutClusterList));

        return [.. dataList];
    }

    private static IEnumerable<byte> GetBytes(ushort[] data) => data.SelectMany(BitConverter.GetBytes);
}
