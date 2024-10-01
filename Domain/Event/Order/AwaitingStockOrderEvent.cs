using MediatR;

namespace Domain.Event.Order;

public class AwaitingStockOrderEvent : INotification
{
    public Guid OrderId { get; private set; }
    public List<Guid> ProductsWithoutStock { get; private set; }

    public AwaitingStockOrderEvent(Guid orderId, List<Guid> productsWithoutStock)
    {
        OrderId = orderId;
        ProductsWithoutStock = productsWithoutStock;
    }
}
