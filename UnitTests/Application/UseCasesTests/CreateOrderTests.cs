using Application.UseCases.Order.CreateOrder;
using Domain.Entity;
using Domain.Repository;
using MediatR;
using Moq;
using Xunit;

namespace UnitTests.Application.UseCasesTests;

public class CreateOrderTests
{
    [Fact]
    public async Task Handle_ShouldCreateOrder_WhenItemsAreValid()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();
        var mockOrderRepository = new Mock<IOrderRepository>();
        var mockMediator = new Mock<IMediator>();
        var cancellationToken = CancellationToken.None;
        var items = new List<ItemDomain>
        {
            new (Guid.NewGuid(), "Item 1", 100m, 10),
            new (Guid.NewGuid(), "Item 2", 50m, 5)
        };
        mockItemRepository.Setup(repo => repo.GetItemsByIdsAsync(It.IsAny<List<Guid>>(), cancellationToken))
            .ReturnsAsync(items);
        mockOrderRepository.Setup(repo => repo.SaveAsync(It.IsAny<OrderDomain>(), cancellationToken))
            .Returns(Task.CompletedTask);
        var useCase = new CreateOrder(mockItemRepository.Object, mockOrderRepository.Object, mockMediator.Object);
        var request = new CreateOrderInput(Guid.NewGuid(),
            [
                new (items[0].Id, 2),
                new (items[1].Id, 3 )
            ]);

        // Act
        var result = await useCase.Handle(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.OrderId);
        Assert.Equal(350, result.Value.TotalPrice);
        mockItemRepository.Verify(repo => repo.GetItemsByIdsAsync(It.IsAny<List<Guid>>(), cancellationToken), Times.Once);
        mockOrderRepository.Verify(repo => repo.SaveAsync(It.IsAny<OrderDomain>(), cancellationToken), Times.Once);
        mockMediator.Verify(m => m.Publish(It.IsAny<INotification>(), cancellationToken), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenItemStockIsInsufficient()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();
        var mockOrderRepository = new Mock<IOrderRepository>();
        var mockMediator = new Mock<IMediator>();
        var cancellationToken = CancellationToken.None;
        var items = new List<ItemDomain>
        {
            new(Guid.NewGuid(), "Item 1", 100m, 1)
        };
        mockItemRepository.Setup(repo => repo.GetItemsByIdsAsync(It.IsAny<List<Guid>>(), cancellationToken))
            .ReturnsAsync(items);
        var useCase = new CreateOrder(mockItemRepository.Object, mockOrderRepository.Object, mockMediator.Object);
        var request = new CreateOrderInput(Guid.NewGuid(), [new(items[0].Id, 2)]);

        // Act
        var result = await useCase.Handle(request, cancellationToken);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Not enough stock", result.Errors[0].Message);
        mockOrderRepository.Verify(repo => repo.SaveAsync(It.IsAny<OrderDomain>(), cancellationToken), Times.Never);
        mockMediator.Verify(m => m.Publish(It.IsAny<INotification>(), cancellationToken), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenExceptionIsThrown()
    {
        // Arrange
        var mockItemRepository = new Mock<IItemRepository>();
        var mockOrderRepository = new Mock<IOrderRepository>();
        var mockMediator = new Mock<IMediator>();
        var cancellationToken = CancellationToken.None;
        mockItemRepository.Setup(repo => repo.GetItemsByIdsAsync(It.IsAny<List<Guid>>(), cancellationToken))
            .ThrowsAsync(new Exception("Database error"));
        var useCase = new CreateOrder(mockItemRepository.Object, mockOrderRepository.Object, mockMediator.Object);
        var request = new CreateOrderInput(Guid.NewGuid(), [new(Guid.NewGuid(), 1)]);

        // Act
        var result = await useCase.Handle(request, cancellationToken);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("Database error", result.Errors[0].Message);
        mockOrderRepository.Verify(repo => repo.SaveAsync(It.IsAny<OrderDomain>(), cancellationToken), Times.Never);
        mockMediator.Verify(m => m.Publish(It.IsAny<INotification>(), cancellationToken), Times.Never);
    }
}
