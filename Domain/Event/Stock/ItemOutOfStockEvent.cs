using MediatR;

namespace Domain.Event.Stock;

public class ItemOutOfStockEvent : INotification
{
    public Guid ItemId { get; private set; }
    public int QuantityRequested { get; private set; }

    public ItemOutOfStockEvent(Guid itemId, int quantityRequested)
    {
        ItemId = itemId;
        QuantityRequested = quantityRequested;
    }
}
