using Domain.Entity;

namespace Application.UseCases.Transaction.TransactionMethod;

public interface ITransaction
{
    Task<(bool IsSuccessful, decimal FinalAmount)> ProcessTransaction(OrderDomain order);
    bool CanRefund();
    Task Refund(Guid transactionId);
}
