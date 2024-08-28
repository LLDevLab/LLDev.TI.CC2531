﻿namespace LLDev.TI.CC2531.RxTx.Configs;

internal sealed class SerialPortHandlerConfig
{
    public string PortName { get; init; } = string.Empty;
    public int BaudRate { get; init; } = 115200;
    public int ReadWriteTimeoutMs { get; init; } = 3000;
}
