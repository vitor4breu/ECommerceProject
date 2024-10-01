using Domain.Enum;

namespace Domain.Entity;

public class TransactionDomain
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public TransactionMethodEnum Method { get; private set; }
    public TransactionStatusEnum Status { get; private set; }

    public TransactionDomain() { }

    public TransactionDomain(Guid orderId, decimal amout, TransactionMethodEnum method, TransactionStatusEnum status)
    {
        Id = new Guid();
        OrderId = orderId;
        Amount = amout;
        Method = method;
        Status = status;
    }
}
