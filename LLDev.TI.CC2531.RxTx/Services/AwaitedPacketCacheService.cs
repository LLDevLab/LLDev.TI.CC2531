using LLDev.TI.CC2531.RxTx.Enums;

namespace LLDev.TI.CC2531.RxTx.Services;

internal interface IAwaitedPacketCacheService
{
    void Add(ZToolCmdType packetType);
    bool Contains(ZToolCmdType packetType);
    void Remove(ZToolCmdType packetType);
}

internal sealed class AwaitedPacketCacheService : IAwaitedPacketCacheService
{
    private readonly HashSet<ZToolCmdType> _awaitedPacketTypes = [];

    private readonly object _lock = new();

    public void Add(ZToolCmdType packetType)
    {
        lock (_lock)
        {
            if (_awaitedPacketTypes.Contains(packetType))
                throw new InvalidOperationException($"Packet of type {packetType} is already awaiting");

            _awaitedPacketTypes.Add(packetType);
        }
    }

    public bool Contains(ZToolCmdType packetType)
    {
        bool result;

        lock (_lock)
        {
            result = _awaitedPacketTypes.Contains(packetType);
        }

        return result;
    }

    public void Remove(ZToolCmdType packetType)
    {
        lock (_lock)
        {
            if (!_awaitedPacketTypes.Contains(packetType))
                return;

            _awaitedPacketTypes.Remove(packetType);
        }
    }
}
