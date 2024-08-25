using LLDev.TI.CC2531.RxTx.Packets.Incoming;

namespace LLDev.TI.CC2531.RxTx;

public delegate void SerialPortDataReceivedEventHandler();
public delegate void MessageReceivedHandler(IIncomingPacket packet);
