using Domain.Repository;
using FluentResults;

namespace Application.UseCases.Order.GetOrderStatus;

public class GetOrderStatus : IGetOrderStatus
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderStatus(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<GetOrderStatusOutput>> Handle(GetOrderStatusInput request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
                return Result.Fail($"Order with ID {request.OrderId} not found.");

            var orderOutput = new GetOrderStatusOutput(
                order.Id,
                order.State,
                order.Total,
                order.Items.Select(i => new ItemOutput(i.Item.Id, i.Item.Name, i.Item.Price, i.Quantity)).ToList());

            return Result.Ok(orderOutput);
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
