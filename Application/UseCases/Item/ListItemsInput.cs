using FluentResults;
using MediatR;

namespace Application.UseCases.Item;

public class ListItemsInput : IRequest<Result<IReadOnlyList<ListItemsOutput>>>
{
}
