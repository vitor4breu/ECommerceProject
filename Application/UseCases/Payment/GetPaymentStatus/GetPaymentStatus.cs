using Domain.Repository;
using FluentResults;

namespace Application.UseCases.Transaction.GetTransactionStatus;

public class GetTransactionStatus : IGetTransactionStatus
{
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionStatus(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<GetTransactionStatusOutput>> Handle(GetTransactionStatusInput request, CancellationToken cancellationToken)
    {
        try
        {
            var transaction = await _transactionRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);

            if (transaction is null)
                return Result.Fail($"Transaction with OrderId {request.OrderId} not found.");

            return Result.Ok(new GetTransactionStatusOutput(transaction.Status));
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
