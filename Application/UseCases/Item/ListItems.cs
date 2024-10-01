using Domain.Repository;
using FluentResults;

namespace Application.UseCases.Item;

public class ListItems : IListItems
{
    private readonly IItemRepository _repository;

    public ListItems(IItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<ListItemsOutput>>> Handle(ListItemsInput input, CancellationToken cancellationToken)
    {
        try
        {
            var items = await _repository.GetAsync(cancellationToken);

            var output = items.Select(item => new ListItemsOutput(
                item.Id,
                item.Name,
                item.Price,
                item.QuantityInStock
            )).ToList();

            return Result.Ok<IReadOnlyList<ListItemsOutput>>(output);
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
