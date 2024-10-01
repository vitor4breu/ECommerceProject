using Domain.Entity;

namespace Domain.Repository;

public interface IItemRepository
{
    Task<IReadOnlyList<ItemDomain>> GetItemsByIdsAsync(List<Guid> ids, CancellationToken cancellationToken);
    Task<IReadOnlyList<ItemDomain?>> GetAsync(CancellationToken cancellationToken);
}
