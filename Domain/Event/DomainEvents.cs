using MediatR;
using System.Collections.Concurrent;

namespace Domain.Event;

public static class DomainEvents
{
    private static readonly ConcurrentDictionary<Guid, List<INotification>> _notifications = new();

    public static void Raise(INotification eventItem)
    {
        var key = Guid.NewGuid();

        if (!_notifications.ContainsKey(key))
            _notifications[key] = [];

        _notifications[key].Add(eventItem);
    }

    public static async Task DispatchNotifications(IMediator mediator)
    {
        foreach (var key in _notifications.Keys)
        {
            var events = _notifications[key];
            foreach (var domainEvent in events)
            {
                await mediator.Publish(domainEvent);
            }
        }

        _notifications.Clear();
    }
}
