using Application.Interfaces;
using Domain.Entity;
using Polly;

namespace Application.UseCases.Transaction.TransactionMethod;

public class CreditoTransaction : ITransaction
{
    private readonly ITransactionProcessor _transactionGateway;

    public CreditoTransaction(ITransactionProcessor transactionGateway)
    {
        _transactionGateway = transactionGateway;
    }

    public async Task<(bool IsSuccessful, decimal FinalAmount)> ProcessTransaction(OrderDomain order)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .RetryAsync(3);

        var result = await retryPolicy.ExecuteAsync(async () =>
        {
            bool success = await _transactionGateway.ProcessTransactionAsync(order.Total);
            return success;
        });

        return (result, order.Total);
    }

    public bool CanRefund() => true;

    public async Task Refund(Guid transactionId)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .RetryAsync(3);

        await retryPolicy.ExecuteAsync(async () =>
        {
            await _transactionGateway.RefundTransactionAsync(transactionId);
        });
    }
}
