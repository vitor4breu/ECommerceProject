using Domain.Entity;
using Xunit;

namespace UnitTests.Domain.EntityTests;

public class ItemDomainTests
{
    [Fact]
    public void CanFulfillOrder_ReturnsTrue_WhenStockIsGreaterThanOrEqualToRequested()
    {
        // Arrange
        var item = new ItemDomain(Guid.NewGuid(), "Test Item", 10.0m, 5);

        // Act
        var result = item.CanFulfillOrder(3);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanFulfillOrder_ReturnsFalse_WhenStockIsLessThanRequested()
    {
        // Arrange
        var item = new ItemDomain(Guid.NewGuid(), "Test Item", 10.0m, 2);

        // Act
        var result = item.CanFulfillOrder(3);

        // Assert
        Assert.False(result);
    }
}
