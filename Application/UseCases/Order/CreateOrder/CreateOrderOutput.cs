namespace Application.UseCases.Order.CreateOrder;

public record CreateOrderOutput(Guid OrderId, decimal TotalPrice);