using Domain.Enum;
using MediatR;

namespace Domain.Event.Transaction;

public class CompletedTransactionEvent : INotification
{
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public TransactionMethodEnum TransactionMethod { get; private set; }

    public CompletedTransactionEvent(Guid orderId, decimal amount, TransactionMethodEnum transactionMethod)
    {
        OrderId = orderId;
        Amount = amount;
        TransactionMethod = transactionMethod;
    }
}
