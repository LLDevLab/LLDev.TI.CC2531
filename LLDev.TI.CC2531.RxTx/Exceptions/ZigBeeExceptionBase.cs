namespace LLDev.TI.CC2531.RxTx.Exceptions;
public class ZigBeeExceptionBase : ApplicationException
{
    protected ZigBeeExceptionBase()
    {
    }

    protected ZigBeeExceptionBase(string message) :
        base(message)
    {
    }

    protected ZigBeeExceptionBase(string message, Exception inner) :
        base(message, inner)
    {
    }
}
