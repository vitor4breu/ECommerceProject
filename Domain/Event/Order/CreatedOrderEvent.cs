using MediatR;

namespace Domain.Event.Order;

public class CreatedOrderEvent : INotification
{
    public Guid OrderId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public CreatedOrderEvent(Guid orderId)
    {
        OrderId = orderId;
        CreatedAt = DateTime.Now;
    }
}
