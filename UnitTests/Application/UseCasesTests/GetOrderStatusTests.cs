using Application.UseCases.Order.GetOrderStatus;
using Domain.Entity;
using Domain.Repository;
using Moq;
using Xunit;

namespace UnitTests.Application.UseCasesTests;

public class GetOrderStatusTests
{
    [Fact]
    public async Task Handle_ShouldReturnOrderStatus_WhenOrderExists()
    {
        // Arrange
        var mockOrderRepository = new Mock<IOrderRepository>();
        var cancellationToken = CancellationToken.None;
        var item1 = new ItemDomain(Guid.NewGuid(), "Item 1", 100m, 10);
        var item2 = new ItemDomain(Guid.NewGuid(), "Item 2", 100m, 10);
        var order = new OrderDomain(Guid.NewGuid());
        order.Create(
        [
            (item1, 1),
            (item2, 1)
        ]);
        order.CompleteTransaction(200m);
        mockOrderRepository.Setup(repo => repo.GetByIdAsync(order.Id, cancellationToken))
            .ReturnsAsync(order);
        var useCase = new GetOrderStatus(mockOrderRepository.Object);
        var request = new GetOrderStatusInput(order.Id);

        // Act
        var result = await useCase.Handle(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(order.Id, result.Value.Id);
        Assert.Equal(order.State, result.Value.Status);
        Assert.Equal(order.Total, result.Value.Amout);
        Assert.Equal(order.Items.Count, result.Value.Items.Count);
        mockOrderRepository.Verify(repo => repo.GetByIdAsync(order.Id, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenOrderNotFound()
    {
        // Arrange
        var mockOrderRepository = new Mock<IOrderRepository>();
        var cancellationToken = CancellationToken.None;
        mockOrderRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), cancellationToken))
            .ReturnsAsync((OrderDomain)null);
        var useCase = new GetOrderStatus(mockOrderRepository.Object);
        var request = new GetOrderStatusInput(Guid.NewGuid());

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
        var cancellationToken = CancellationToken.None;
        mockOrderRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), cancellationToken))
            .ThrowsAsync(new Exception("Database error"));
        var useCase = new GetOrderStatus(mockOrderRepository.Object);
        var request = new GetOrderStatusInput(Guid.NewGuid());

        // Act
        var result = await useCase.Handle(request, cancellationToken);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("Database error", result.Errors[0].Message);
        mockOrderRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>(), cancellationToken), Times.Once);
    }
}
