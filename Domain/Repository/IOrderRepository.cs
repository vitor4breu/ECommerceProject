using Domain.Entity;

namespace Domain.Repository;

public interface IOrderRepository
{
    Task<OrderDomain?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<OrderDomain> UpdateAsync(OrderDomain order, CancellationToken cancellationToken);
    Task SaveAsync(OrderDomain order, CancellationToken cancellationToken);
}
