using Domain.Entity;

namespace Domain.Repository;

public interface ITransactionRepository
{
    Task<TransactionDomain?> GetByOrderIdAsync(Guid id, CancellationToken cancellationToken);
    Task SaveAsync(TransactionDomain transaction, CancellationToken cancellationToken);
}
