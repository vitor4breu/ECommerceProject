using Application.Interfaces;

namespace Infrastructure.TransactionProcessor;

public class TransactionProcessorMoq : ITransactionProcessor
{
    public async Task<bool> ProcessTransactionAsync(decimal value)
    {
        await Task.Delay(100);

        return true;
    }

    public async Task RefundTransactionAsync(Guid transactionId)
    {
        await Task.Delay(100);
    }
}
