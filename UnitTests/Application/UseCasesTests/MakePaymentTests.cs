using Application.Interfaces;
using Application.UseCases.Transaction.MakeTransaction;
using Domain.Entity;
using Domain.Enum;
using Domain.Repository;
using MediatR;
using Moq;
using Xunit;

namespace UnitTests.Application.UseCasesTests;

public class MakeTransactionTests
{
    [Fact]
    public async Task Handle_ShouldReturnError_WhenOrderNotFound()
    {
        // Arrange
        var mockOrderRepository = new Mock<IOrderRepository>();
        var mockTransactionRepository = new Mock<ITransactionRepository>();
        var mockMediator = new Mock<IMediator>();
        var mockTransactionGateway = new Mock<ITransactionProcessor>();
        var makeTransaction = new MakeTransaction(mockOrderRepository.Object, mockTransactionRepository.Object, mockMediator.Object, mockTransactionGateway.Object);
        var request = new MakeTransactionInput(Guid.NewGuid(), TransactionMethodEnum.Credito);
        mockOrderRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderDomain)null);

        // Act
        var result = await makeTransaction.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal($"Order with ID {request.OrderId} not found.", result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenOrderIsNotInCorrectState()
    {
        // Arrange
        var mockOrderRepository = new Mock<IOrderRepository>();
        var mockTransactionRepository = new Mock<ITransactionRepository>();
        var mockMediator = new Mock<IMediator>();
        var mockTransactionGateway = new Mock<ITransactionProcessor>();
        var makeTransaction = new MakeTransaction(mockOrderRepository.Object, mockTransactionRepository.Object, mockMediator.Object, mockTransactionGateway.Object);
        var request = new MakeTransactionInput(Guid.NewGuid(), TransactionMethodEnum.Credito);
        var item1 = new ItemDomain(Guid.NewGuid(), "Item 1", 100m, 10);
        var item2 = new ItemDomain(Guid.NewGuid(), "Item 2", 100m, 10);
        var order = new OrderDomain(Guid.NewGuid());
        order.Create(
        [
            (item1, 1),
            (item2, 1)
        ]);
        order.Cancel();
        mockOrderRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await makeTransaction.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("Order must be in 'Aguardando Processamento' state to process transaction.", result.Errors.First().Message);
    }

    [Fact]
    public async Task Handle_ShouldProcessTransactionSuccessfully_WhenTransactionIsApproved()
    {
        // Arrange
        var mockOrderRepository = new Mock<IOrderRepository>();
        var mockTransactionRepository = new Mock<ITransactionRepository>();
        var mockMediator = new Mock<IMediator>();
        var mockTransactionGateway = new Mock<ITransactionProcessor>();
        var makeTransaction = new MakeTransaction(mockOrderRepository.Object, mockTransactionRepository.Object, mockMediator.Object, mockTransactionGateway.Object);
        var request = new MakeTransactionInput(Guid.NewGuid(), TransactionMethodEnum.Credito);
        var item1 = new ItemDomain(Guid.NewGuid(), "Item 1", 100m, 10);
        var item2 = new ItemDomain(Guid.NewGuid(), "Item 2", 100m, 10);
        var order = new OrderDomain(Guid.NewGuid());
        order.Create(
        [
            (item1, 1),
            (item2, 1)
        ]);
        mockOrderRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        mockTransactionGateway.Setup(gateway => gateway.ProcessTransactionAsync(It.IsAny<decimal>()))
            .ReturnsAsync(true); // Simula um pagamento bem-sucedido

        // Act
        var result = await makeTransaction.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(order.Id, result.Value.TransactionId);
        mockTransactionRepository.Verify(repo => repo.SaveAsync(It.IsAny<TransactionDomain>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
