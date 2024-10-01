using MediatR;

namespace Domain.Event.Stock;

public class StockDecreasedEvent : INotification
{
    public Guid ItemId { get; private set; }
    public int QuantityDecresed { get; private set; }
    public DateTime DecresedAt { get; private set; }

    public StockDecreasedEvent(Guid itemId, int quantityDecresed)
    {
        ItemId = itemId;
        QuantityDecresed = quantityDecresed;
        DecresedAt = DateTime.Now;
    }
}
