using Domain.Entity;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly EcommerceDbContext _dbContext;

    public TransactionRepository(EcommerceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TransactionDomain?> GetByOrderIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.OrderId == id, cancellationToken);
    }

    public async Task SaveAsync(TransactionDomain transaction, CancellationToken cancellationToken)
    {
        await _dbContext.Transactions.AddAsync(transaction, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
