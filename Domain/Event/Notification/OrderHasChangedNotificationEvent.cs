using Domain.Enum;
using MediatR;

namespace Domain.Event.Notification;

public class OrderHasChangedNotificationEvent : INotification
{
    public Guid OrderId { get; private set; }
    public OrderStateEnum OrderState { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTime ChangedAt { get; private set; }

    public OrderHasChangedNotificationEvent(Guid orderId, OrderStateEnum orderState, Guid customerId)
    {
        OrderId = orderId;
        OrderState = orderState;
        CustomerId = customerId;
        ChangedAt = DateTime.Now;
    }
}
