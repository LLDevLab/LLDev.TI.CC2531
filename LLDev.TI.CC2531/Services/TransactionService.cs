namespace LLDev.TI.CC2531.Services;

internal interface ITransactionService
{
    byte GetNextTransactionId();
}

internal sealed class TransactionService : ITransactionService
{
    private byte _nextTransactionId = 0;
    private readonly object _objectLock = new();

    public byte GetNextTransactionId()
    {
        byte result;

        lock (_objectLock)
        {
            _nextTransactionId++;
            result = _nextTransactionId;
        }

        return result;
    }
}
