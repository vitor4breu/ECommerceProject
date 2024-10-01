using Domain.Entity;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly EcommerceDbContext _dbContext;

    public OrderRepository(EcommerceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OrderDomain?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Item)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task SaveAsync(OrderDomain order, CancellationToken cancellationToken)
    {
        await _dbContext.Orders.AddAsync(order, cancellationToken);
        foreach (var orderItem in order.Items)
            _dbContext.Attach(orderItem.Item);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<OrderDomain> UpdateAsync(OrderDomain order, CancellationToken cancellationToken)
    {
        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return order;
    }
}
