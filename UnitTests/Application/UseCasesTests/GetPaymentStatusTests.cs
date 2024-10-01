using Application.UseCases.Transaction.GetTransactionStatus;
using Domain.Entity;
using Domain.Enum;
using Domain.Repository;
using Moq;
using Xunit;

namespace UnitTests.Application.UseCasesTests;

public class GetTransactionStatusTests
{
    [Fact]
    public async Task Handle_ShouldReturnTransactionStatus_WhenTransactionExists()
    {
        // Arrange
        var mockTransactionRepository = new Mock<ITransactionRepository>();
        var cancellationToken = CancellationToken.None;
        var transaction = new TransactionDomain(Guid.NewGuid(), 100m, TransactionMethodEnum.Credito, TransactionStatusEnum.Aprovado);
        mockTransactionRepository.Setup(repo => repo.GetByOrderIdAsync(transaction.OrderId, cancellationToken))
            .ReturnsAsync(transaction);
        var useCase = new GetTransactionStatus(mockTransactionRepository.Object);
        var request = new GetTransactionStatusInput(transaction.OrderId);

        // Act
        var result = await useCase.Handle(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(transaction.Status, result.Value.Status);
        mockTransactionRepository.Verify(repo => repo.GetByOrderIdAsync(transaction.OrderId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenTransactionNotFound()
    {
        // Arrange
        var mockTransactionRepository = new Mock<ITransactionRepository>();
        var cancellationToken = CancellationToken.None;
        mockTransactionRepository.Setup(repo => repo.GetByOrderIdAsync(It.IsAny<Guid>(), cancellationToken))
            .ReturnsAsync((TransactionDomain)null);
        var useCase = new GetTransactionStatus(mockTransactionRepository.Object);
        var request = new GetTransactionStatusInput(Guid.NewGuid());

        // Act
        var result = await useCase.Handle(request, cancellationToken);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("not found", result.Errors[0].Message);
        mockTransactionRepository.Verify(repo => repo.GetByOrderIdAsync(It.IsAny<Guid>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenExceptionIsThrown()
    {
        // Arrange
        var mockTransactionRepository = new Mock<ITransactionRepository>();
        var cancellationToken = CancellationToken.None;
        mockTransactionRepository.Setup(repo => repo.GetByOrderIdAsync(It.IsAny<Guid>(), cancellationToken))
            .ThrowsAsync(new Exception("Database error"));
        var useCase = new GetTransactionStatus(mockTransactionRepository.Object);
        var request = new GetTransactionStatusInput(Guid.NewGuid());

        // Act
        var result = await useCase.Handle(request, cancellationToken);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("Database error", result.Errors[0].Message);
        mockTransactionRepository.Verify(repo => repo.GetByOrderIdAsync(It.IsAny<Guid>(), cancellationToken), Times.Once);
    }
}
