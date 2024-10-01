using FluentResults;
using MediatR;

namespace Application.UseCases.Order.CreateOrder;

public record CreateOrderInput(Guid CustomerId, List<ItemsInput> Items) : IRequest<Result<CreateOrderOutput>>;

public record ItemsInput(Guid Id, uint Quantity);