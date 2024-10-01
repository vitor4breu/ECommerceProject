using MediatR;

namespace Domain.Event.Order;

public class CompletedOrderEvent : INotification
{
    public Guid OrderId { get; private set; }
    public DateTime CompletedAt { get; private set; }

    public CompletedOrderEvent(Guid orderId)
    {
        OrderId = orderId;
        CompletedAt = DateTime.Now;
    }
}
