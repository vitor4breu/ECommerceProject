using FluentResults;
using MediatR;

namespace Application.UseCases.Order.CancelOrder;

public record CancelOrderInput(Guid Id) : IRequest<Result>;
