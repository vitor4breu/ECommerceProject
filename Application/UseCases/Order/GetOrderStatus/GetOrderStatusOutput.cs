using Domain.Enum;

namespace Application.UseCases.Order.GetOrderStatus;

public record GetOrderStatusOutput(Guid Id, OrderStateEnum Status, decimal Amout, List<ItemOutput> Items);