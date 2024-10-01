using Application.UseCases.Item;
using Domain.Entity;
using Domain.Repository;
using Moq;
using Xunit;

namespace UnitTests.Application.UseCasesTests;

public class ListItemsTests
{
    [Fact]
    public async Task Handle_ShouldReturnListOfItems_WhenItemsExist()
    {
        // Arrange
        var mockRepository = new Mock<IItemRepository>();
        var cancellationToken = CancellationToken.None;
        var items = new List<ItemDomain>
        {
            new ItemDomain(Guid.NewGuid(), "Item 1", 100m, 10),
            new ItemDomain(Guid.NewGuid(), "Item 2", 50m, 5)
        };
        mockRepository.Setup(repo => repo.GetAsync(cancellationToken))
            .ReturnsAsync(items);
        var useCase = new ListItems(mockRepository.Object);
        var input = new ListItemsInput();

        // Act
        var result = await useCase.Handle(input, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal("Item 1", result.Value[0].Name);
        Assert.Equal(100m, result.Value[0].Price);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenExceptionIsThrown()
    {
        // Arrange
        var mockRepository = new Mock<IItemRepository>();
        var cancellationToken = CancellationToken.None;
        mockRepository.Setup(repo => repo.GetAsync(cancellationToken))
            .ThrowsAsync(new Exception("Database error"));
        var useCase = new ListItems(mockRepository.Object);
        var input = new ListItemsInput();

        // Act
        var result = await useCase.Handle(input, cancellationToken);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal("Database error", result.Errors[0].Message);
    }
}
