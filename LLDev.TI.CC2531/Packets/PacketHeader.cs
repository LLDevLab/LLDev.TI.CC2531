﻿using LLDev.TI.CC2531.Enums;

namespace LLDev.TI.CC2531.Packets;

internal interface IPacketHeader
{
    byte StartByte { get; }
    byte DataLength { get; }
    ZToolCmdType CmdType { get; }

    public byte[] ToByteArray();
}

internal sealed class PacketHeader(byte[] data) : IPacketHeader
{
    public byte StartByte => _data[0];
    public byte DataLength => _data[1];
    public ZToolCmdType CmdType
    {
        get
        {
            var cmd = (ushort)((_data[2] << 8) | _data[3]);
            return Enum.IsDefined(typeof(ZToolCmdType), cmd) ? (ZToolCmdType)cmd : ZToolCmdType.Unknown;
        }
    }

    private readonly byte[] _data = data[0..Constants.HeaderLength];

    public byte[] ToByteArray() => _data;
}
