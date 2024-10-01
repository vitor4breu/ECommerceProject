namespace Application.Interfaces;

public interface ITransactionProcessor
{
    Task<bool> ProcessTransactionAsync(decimal amount);
    Task RefundTransactionAsync(Guid transactionId);
}
