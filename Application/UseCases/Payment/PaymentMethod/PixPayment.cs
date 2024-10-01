using Application.Interfaces;
using Domain.Entity;
using Polly;

namespace Application.UseCases.Transaction.TransactionMethod;

public class PixTransaction : ITransaction
{
    private readonly ITransactionProcessor _transactionGateway;

    public PixTransaction(ITransactionProcessor transactionGateway)
    {
        _transactionGateway = transactionGateway;
    }

    public async Task<(bool IsSuccessful, decimal FinalAmount)> ProcessTransaction(OrderDomain order)
    {
        var pixValue = order.Total * 0.95m;

        var retryPolicy = Policy
            .Handle<Exception>()
            .RetryAsync(3);

        var result = await retryPolicy.ExecuteAsync(async () =>
        {
            bool success = await _transactionGateway.ProcessTransactionAsync(pixValue);
            return success;
        });

        return (result, pixValue);
    }

    public bool CanRefund() => false;

    public Task Refund(Guid transactionId)
    {
        throw new InvalidOperationException("Cant`t refund Pix.");
    }
}
