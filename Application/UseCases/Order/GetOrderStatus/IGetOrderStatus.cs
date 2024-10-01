using FluentResults;
using MediatR;

namespace Application.UseCases.Order.GetOrderStatus;

internal interface IGetOrderStatus : IRequestHandler<GetOrderStatusInput, Result<GetOrderStatusOutput>>
{
}
