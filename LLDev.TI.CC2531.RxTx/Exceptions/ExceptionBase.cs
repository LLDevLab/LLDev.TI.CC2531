namespace LLDev.TI.CC2531.RxTx.Exceptions;
public class ExceptionBase : ApplicationException
{
    protected ExceptionBase()
    {
    }

    protected ExceptionBase(string message) :
        base(message)
    {
    }

    protected ExceptionBase(string message, Exception inner) :
        base(message, inner)
    {
    }
}
