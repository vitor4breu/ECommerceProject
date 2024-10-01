using Application.Interfaces;
using Domain.Entity;
using Polly;

namespace Application.UseCases.Transaction.TransactionMethod;

public class BoletoTransaction : ITransaction
{
    private readonly ITransactionProcessor _transactionGateway;

    public BoletoTransaction(ITransactionProcessor transactionGateway)
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

    public bool CanRefund() => false;

    public Task Refund(Guid transactionId)
    {
        throw new InvalidOperationException("Cant`t refund Boleto.");
    }
}
