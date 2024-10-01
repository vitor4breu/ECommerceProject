using Domain.Event;
using Domain.Repository;
using FluentResults;
using MediatR;

namespace Application.UseCases.Order.CancelOrder;

public class CancelOrder : ICancelOrder
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMediator _mediator;

    public CancelOrder(IOrderRepository orderRepository, IMediator mediator)
    {
        _orderRepository = orderRepository;
        _mediator = mediator;
    }

    public async Task<Result> Handle(CancelOrderInput request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);

            if (order is null)
                return Result.Fail($"Order with ID {request.Id} not found.");

            order.Cancel();

            await _orderRepository.UpdateAsync(order, cancellationToken);
            await DomainEvents.DispatchNotifications(_mediator);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
