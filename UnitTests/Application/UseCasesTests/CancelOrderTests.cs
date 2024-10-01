using Application.UseCases.Order.CancelOrder;
using Domain.Entity;
using Domain.Enum;
using Domain.Repository;
using MediatR;
using Moq;
using Xunit;

namespace UnitTests.Application.UseCasesTests;

public class CancelOrderTests
{
    [Fact]
    public async Task Handle_ShouldCancelOrder_WhenOrderExists()
    {
        // Arrange
        var mockOrderRepository = new Mock<IOrderRepository>();
        var mockMediator = new Mock<IMediator>();
        var cancellationToken = CancellationToken.None;
        var order = new OrderDomain(Guid.NewGuid());
        order.Create(new List<(ItemDomain item, uint quantity)> { (new ItemDomain(Guid.NewGuid(), "Item 1", 100m, 10), 1) });
        mockOrderRepository.Setup(repo => repo.GetByIdAsync(order.Id, cancellationToken))
            .ReturnsAsync(order);
        var useCase = new CancelOrder(mockOrderRepository.Object, mockMediator.Object);
        var request = new CancelOrderInput(order.Id);

        // Act
        var result = await useCase.Handle(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStateEnum.Cancelado, order.State);
        mockOrderRepository.Verify(repo => repo.UpdateAsync(order, cancellationToken), Times.Once);
        mockMediator.Verify(m => m.Publish(It.IsAny<INotification>(), cancellationToken), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenOrderNotFound()
    {
        // Arrange
        var mockOrderRepository = new Mock<IOrderRepository>();
        var mockMediator = new Mock<IMediator>();
        var cancellationToken = CancellationToken.None;
        mockOrderRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), cancellationToken))
            .ReturnsAsync((OrderDomain)null);
        var useCase = new CancelOrder(mockOrderRepository.Object, mockMediator.Object);
        var request = new CancelOrderInput(Guid.NewGuid());

        // Act
        var result = await useCase.Handle(request, cancellationToken);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("not found", result.Errors[0].Message);
        mockOrderRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenExceptionIsThrown()
    {
        // Arrange
        var mockOrderRepository = new Mock<IOrderRepository>();
        var mockMediator = new Mock<IMediator>();
        var cancellationToken = CancellationToken.None;
        mockOrderRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), cancellationToken))
            .ThrowsAsync(new Exception("Database error"));
        var useCase = new CancelOrder(mockOrderRepository.Object, mockMediator.Object);
        var request = new CancelOrderInput(Guid.NewGuid());

        // Act
        var result = await useCase.Handle(request, cancellationToken);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("Database error", result.Errors[0].Message);
        mockOrderRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>(), cancellationToken), Times.Once);
    }
}
