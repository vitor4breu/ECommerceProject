using FluentResults;
using MediatR;

namespace Application.UseCases.Order.CreateOrder;

public interface ICreateOrder : IRequestHandler<CreateOrderInput, Result<CreateOrderOutput>>
{
}
