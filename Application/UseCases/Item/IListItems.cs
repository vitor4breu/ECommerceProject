using FluentResults;
using MediatR;

namespace Application.UseCases.Item;

public interface IListItems : IRequestHandler<ListItemsInput, Result<IReadOnlyList<ListItemsOutput>>>
{
}
