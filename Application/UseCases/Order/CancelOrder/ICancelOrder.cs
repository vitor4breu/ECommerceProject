using FluentResults;
using MediatR;

namespace Application.UseCases.Order.CancelOrder;

public interface ICancelOrder : IRequestHandler<CancelOrderInput, Result>
{
}
