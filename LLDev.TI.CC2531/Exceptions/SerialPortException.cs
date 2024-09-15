namespace LLDev.TI.CC2531.Exceptions;
public sealed class SerialPortException : ExceptionBase
{
    public SerialPortException()
    {
    }

    public SerialPortException(string message) : base(message)
    {
    }

    public SerialPortException(string message, Exception inner) : base(message, inner)
    {
    }
}
