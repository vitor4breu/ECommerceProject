using Domain.Entity;
using Domain.Enum;
using Xunit;

namespace UnitTests.Domain.EntityTests;

public class OrderDomainTests
{
    [Fact]
    public void Create_ShouldCreateOrderWithValidItems()
    {
        var item1 = new ItemDomain(Guid.NewGuid(), "Item 1", 100, 10);
        var item2 = new ItemDomain(Guid.NewGuid(), "Item 2", 200, 20);
        var items = new List<(ItemDomain item, uint quantity)>
        {
            (item1, 5),
            (item2, 3)
        };

        var order = new OrderDomain(Guid.NewGuid());
        order.Create(items);

        Assert.Equal(OrderStateEnum.AguardandoProcessamento, order.State);
        Assert.Equal(1100m, order.Total);
    }

    [Fact]
    public void Create_ShouldThrowException_WhenItemQuantityIsZero()
    {
        var item = new ItemDomain(Guid.NewGuid(), "Item 1", 100, 10);
        var items = new List<(ItemDomain item, uint quantity)>
        {
            (item, 0)
        };

        var order = new OrderDomain(Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(() => order.Create(items));
    }

    [Fact]
    public void Create_ShouldThrowException_WhenNotEnoughStock()
    {
        var item = new ItemDomain(Guid.NewGuid(), "Item 1", 100, 5);
        var items = new List<(ItemDomain item, uint quantity)>
        {
            (item, 10)
        };

        var order = new OrderDomain(Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(() => order.Create(items));
    }

    [Fact]
    public void Create_ShouldSetInitialStateToAwaitingProcessing()
    {
        var order = new OrderDomain(Guid.NewGuid());
        var items = new List<(ItemDomain item, uint quantity)>
        {
            (new ItemDomain(Guid.NewGuid(), "Item 1", 100, 10), 5)
        };

        order.Create(items);

        Assert.Equal(OrderStateEnum.AguardandoProcessamento, order.State);
    }

    [Fact]
    public void Cancel_ShouldCancelOrder_WhenInAwaitingProcessing()
    {
        var order = new OrderDomain(Guid.NewGuid());
        var items = new List<(ItemDomain item, uint quantity)>
        {
            (new ItemDomain(Guid.NewGuid(), "Item 1", 100, 10), 5)
        };

        order.Create(items);
        order.Cancel();

        Assert.Equal(OrderStateEnum.Cancelado, order.State);
    }

    [Fact]
    public void Cancel_ShouldThrowException_WhenNotInAwaitingProcessing()
    {
        var order = new OrderDomain(Guid.NewGuid());
        order.Create(new List<(ItemDomain item, uint quantity)>
        {
            (new ItemDomain(Guid.NewGuid(), "Item 1", 100, 10), 5)
        });
        order.Cancel();

        Assert.Throws<InvalidOperationException>(() => order.Cancel());
    }

    [Fact]
    public void StartTransactionProcess_ShouldChangeStateToProcessingTransaction()
    {
        var order = new OrderDomain(Guid.NewGuid());
        var items = new List<(ItemDomain item, uint quantity)>
        {
            (new ItemDomain(Guid.NewGuid(), "Item 1", 100, 10), 5)
        };

        order.Create(items);
        order.StartTransactionProcess();

        Assert.Equal(OrderStateEnum.ProcessandoPagamento, order.State);
    }

    [Fact]
    public void StartTransactionProcess_ShouldThrowException_WhenNotInAwaitingProcessing()
    {
        var order = new OrderDomain(Guid.NewGuid());
        order.Create(new List<(ItemDomain item, uint quantity)>
        {
            (new ItemDomain(Guid.NewGuid(), "Item 1", 100, 10), 5)
        });
        order.Cancel();

        Assert.Throws<InvalidOperationException>(() => order.StartTransactionProcess());
    }

    [Fact]
    public void CompleteTransaction_ShouldChangeStateToTransactionCompleted()
    {
        var order = new OrderDomain(Guid.NewGuid());
        var items = new List<(ItemDomain item, uint quantity)>
        {
            (new ItemDomain(Guid.NewGuid(), "Item 1", 100, 10), 5)
        };

        order.Create(items);
        order.StartTransactionProcess();
        order.CompleteTransaction(800m);

        Assert.Equal(OrderStateEnum.PagamentoConcluido, order.State);
        Assert.Equal(800m, order.Total);
    }

    [Fact]
    public void FailTransaction_ShouldChangeStateToCancelled()
    {
        var order = new OrderDomain(Guid.NewGuid());
        var items = new List<(ItemDomain item, uint quantity)>
        {
            (new ItemDomain(Guid.NewGuid(), "Item 1", 100, 10), 5)
        };

        order.Create(items);
        order.FailTransaction();

        Assert.Equal(OrderStateEnum.Cancelado, order.State);
    }

    [Fact]
    public void RequestRefund_ShouldChangeStateToRefundRequested()
    {
        var order = new OrderDomain(Guid.NewGuid());
        var items = new List<(ItemDomain item, uint quantity)>
        {
            (new ItemDomain(Guid.NewGuid(), "Item 1", 100, 10), 5)
        };

        order.Create(items);
        order.CompleteTransaction(800m);
        order.RequestRefund();

        Assert.Equal(OrderStateEnum.SolicitadoReembolso, order.State);
    }

    [Fact]
    public void RequestRefund_ShouldThrowException_WhenInInvalidState()
    {
        var order = new OrderDomain(Guid.NewGuid());
        order.Create(new List<(ItemDomain item, uint quantity)>
        {
            (new ItemDomain(Guid.NewGuid(), "Item 1", 100, 10), 5)
        });
        order.Cancel();

        Assert.Throws<InvalidOperationException>(() => order.RequestRefund());
    }

    [Fact]
    public void Refund_ShouldChangeStateToCancelled()
    {
        var order = new OrderDomain(Guid.NewGuid());
        var items = new List<(ItemDomain item, uint quantity)>
        {
            (new ItemDomain(Guid.NewGuid(), "Item 1", 100, 10), 5)
        };

        order.Create(items);
        order.CompleteTransaction(800m);
        order.RequestRefund();
        order.Refund();

        Assert.Equal(OrderStateEnum.Cancelado, order.State);
    }

    [Fact]
    public void NonRefundable_ShouldRevertToPreviousState()
    {
        var order = new OrderDomain(Guid.NewGuid());
        var items = new List<(ItemDomain item, uint quantity)>
        {
            (new ItemDomain(Guid.NewGuid(), "Item 1", 100, 10), 5)
        };

        order.Create(items);
        order.CompleteTransaction(800m);
        order.RequestRefund();
        var oldState = order.State;
        order.NonRefundable(oldState);

        Assert.Equal(oldState, order.State);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenItemsAreEmpty()
    {
        var order = new OrderDomain(Guid.NewGuid());

        Assert.Throws<ArgumentException>(() => order.Create(new List<(ItemDomain item, uint quantity)>()));
    }

    [Fact]
    public void Create_ShouldThrowInvalidOperationException_WhenStockIsInsufficient()
    {
        var items = new List<(ItemDomain item, uint quantity)>
        {
            (new ItemDomain(Guid.NewGuid(), "Item 1", 100, 5), 10)
        };

        var order = new OrderDomain(Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(() => order.Create(items));
    }

    [Fact]
    public void Create_ShouldCalculateTotalWithoutDiscounts()
    {
        var items = new List<(ItemDomain item, uint quantity)>
        {
            (new ItemDomain(Guid.NewGuid(), "Item 1", 100, 10), 2),
            (new ItemDomain(Guid.NewGuid(), "Item 2", 50, 5), 3)
        };

        var order = new OrderDomain(Guid.NewGuid());
        order.Create(items);

        Assert.Equal(350m, order.Total);
    }
}
