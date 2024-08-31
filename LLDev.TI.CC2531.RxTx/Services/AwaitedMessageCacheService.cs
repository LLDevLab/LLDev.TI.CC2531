﻿using LLDev.TI.CC2531.RxTx.Enums;
using LLDev.TI.CC2531.RxTx.Packets.Incoming;
using System.Collections.Concurrent;

namespace LLDev.TI.CC2531.RxTx.Services;

internal interface IAwaitedMessageCacheService
{
    void Add(ZToolCmdType key, Action<IIncomingPacket?> value);
    bool ContainsKey(ZToolCmdType key);
    Action<IIncomingPacket?>? GetAndRemove(ZToolCmdType key);
}

internal sealed class AwaitedMessageCacheService : IAwaitedMessageCacheService
{
    private readonly ConcurrentDictionary<ZToolCmdType, Action<IIncomingPacket?>> _callbackMethods = new();

    public void Add(ZToolCmdType key, Action<IIncomingPacket?> value) => _callbackMethods.TryAdd(key, value);
    public bool ContainsKey(ZToolCmdType key) => _callbackMethods.ContainsKey(key);
    public Action<IIncomingPacket?>? GetAndRemove(ZToolCmdType key)
    {
        return !_callbackMethods.TryRemove(key, out var value)
            ? null
            : value;
    }
}