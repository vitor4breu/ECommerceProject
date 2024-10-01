using Domain.Enum;
using Domain.Event;
using Domain.Event.Notification;

namespace Domain.Entity;

public class OrderDomain
{
    public Guid Id { get; private set; }
    public List<OrderItemDomain> Items { get; private set; }
    public decimal Total { get; private set; }
    public OrderStateEnum State { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid CustomerId { get; private set; }

    public OrderDomain() { }

    public OrderDomain(Guid customerId)
    {
        Id = Guid.NewGuid();
        Items = [];
        CreatedAt = DateTime.Now;
        CustomerId = customerId;
    }

    private static void ValidateItemsStock(List<(ItemDomain item, uint quantity)> items)
    {
        if (items is null || items.Count == 0)
            throw new ArgumentException("Order must contain at least one item.", nameof(items));

        foreach (var (item, quantity) in items)
        {
            if (!item.CanFulfillOrder(quantity))
                throw new InvalidOperationException($"Not enough stock for item {item.Name}. Requested: {quantity}, Available: {item.QuantityInStock}");
        }
    }

    private void CalculateTotalAmount(List<(ItemDomain item, uint quantity)> items)
    {
        Total = items.Sum(i => i.item.Price * i.quantity);

        Total -= ApplyQuantityDiscount(items);
        Total -= ApplySeasonalDiscount(Total);
    }

    private static decimal ApplyQuantityDiscount(List<(ItemDomain item, uint quantity)> items)
    {
        decimal discount = 0;

        foreach (var item in items)
            discount += GetQuantityDiscount(item.quantity);

        return discount;
    }

    private static decimal GetQuantityDiscount(uint quantity)
    {
        if (quantity > 50) return 5m;
        if (quantity > 20) return 3m;
        if (quantity > 10) return 1m;
        return 0;
    }

    private static decimal ApplySeasonalDiscount(decimal total)
    {
        if (DateTime.Now.Month is 12)
            return total * 0.08m;
        return 0;
    }

    public void Create(List<(ItemDomain item, uint quantity)> items)
    {
        if (items.Any(i => i.quantity == 0))
            throw new InvalidOperationException("Order can only be created if all items have more than 1 unit.");
        ValidateItemsStock(items);

        Items = items.Select(i => new OrderItemDomain(i.item, i.quantity)).ToList();
        CalculateTotalAmount(items);
        State = OrderStateEnum.AguardandoProcessamento;

        DomainEvents.Raise(new OrderHasChangedNotificationEvent(Id, State, CustomerId));
    }

    public void Cancel()
    {
        if (State != OrderStateEnum.AguardandoProcessamento)
            throw new InvalidOperationException("Order can only be canceled if it's in 'Aguardando Processamento' state.");

        State = OrderStateEnum.Cancelado;
        DomainEvents.Raise(new OrderHasChangedNotificationEvent(Id, State, CustomerId));
    }

    public void StartTransactionProcess()
    {
        if (State != OrderStateEnum.AguardandoProcessamento)
            throw new InvalidOperationException("Order must be in 'Aguardando Processamento' to start transaction.");

        State = OrderStateEnum.ProcessandoPagamento;
        DomainEvents.Raise(new OrderHasChangedNotificationEvent(Id, State, CustomerId));
    }

    public void CompleteTransaction(decimal finalAmount)
    {
        Total = finalAmount;

        State = OrderStateEnum.PagamentoConcluido;
        DomainEvents.Raise(new OrderHasChangedNotificationEvent(Id, State, CustomerId));
    }

    public void FailTransaction()
    {
        State = OrderStateEnum.Cancelado;
        DomainEvents.Raise(new OrderHasChangedNotificationEvent(Id, State, CustomerId));
    }

    public void RequestRefund()
    {
        if (State != OrderStateEnum.PagamentoConcluido &&
            State != OrderStateEnum.SeparandoPedido &&
            State != OrderStateEnum.AguardandoEstoque)
            throw new InvalidOperationException("Refund can only be requested for orders in or after the 'Pagamento Concluído' state.");

        State = OrderStateEnum.SolicitadoReembolso;
        DomainEvents.Raise(new OrderHasChangedNotificationEvent(Id, State, CustomerId));
    }

    public void Refund()
    {
        if (State != OrderStateEnum.SolicitadoReembolso)
            throw new InvalidOperationException("Refund can only be executed for orders in 'Solicitado Reembolso' state.");

        State = OrderStateEnum.Cancelado;
        DomainEvents.Raise(new OrderHasChangedNotificationEvent(Id, State, CustomerId));
    }

    public void NonRefundable(OrderStateEnum oldState)
    {
        State = oldState;
        DomainEvents.Raise(new OrderHasChangedNotificationEvent(Id, State, CustomerId));
    }
}
