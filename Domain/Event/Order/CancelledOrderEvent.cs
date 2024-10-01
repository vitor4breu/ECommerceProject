using MediatR;

namespace Domain.Event.Order;

public class CancelledOrderEvent : INotification
{
    public Guid OrderId { get; private set; }
    public DateTime CanceledAt { get; private set; }

    public CancelledOrderEvent(Guid orderId)
    {
        OrderId = orderId;
        CanceledAt = DateTime.Now;
    }
}
