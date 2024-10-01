namespace Domain.Entity;

public class OrderItemDomain
{
    public Guid OrderId { get; private set; }
    public Guid ItemId { get; private set; }
    public ItemDomain Item { get; private set; }
    public uint Quantity { get; private set; }

    public OrderItemDomain() { }

    public OrderItemDomain(ItemDomain item, uint quantity)
    {
        Item = item ?? throw new ArgumentNullException(nameof(item));
        Quantity = quantity > 0 ? quantity : throw new InvalidOperationException("Quantity must be greater than zero.");
    }
}
