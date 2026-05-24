using GrabAndGo.Order.Application.Extensions;
using GrabAndGo.Order.Domain.Entities;
using GrabAndGo.Order.Domain.Enums;

namespace GrabAndGo.Order.UnitTests;

public class MappingExtensionsTests
{
    [Fact]
    public void ToDto_Should_MapOrderToOrderDto()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product 
        { 
            Id = productId, 
            Name = "Test Product", 
            Price = 50.0m,
            BusinessId = "b1"
        };
        
        var order = new Domain.Entities.Order
        {
            Id = Guid.NewGuid(),
            OrderNum = "#1234",
            ProductId = productId,
            Product = product,
            TotalAmount = 50.0m,
            Status = OrderStatus.ACCEPTED,
            Date = DateTime.UtcNow,
            UserId = "user-1",
            BusinessId = "b1"
        };

        // Act
        var dto = order.ToDto();

        // Assert
        Assert.Equal(order.Id, dto.Id);
        Assert.Equal(order.OrderNum, dto.OrderNum);
        Assert.Equal(order.Status, dto.Status);
        Assert.Equal(order.TotalAmount, dto.TotalAmount);
        Assert.Equal(product.Name, dto.Product.Name);
    }

    [Fact]
    public void ToDto_Should_HandleNullProduct()
    {
        // Arrange
        var order = new Domain.Entities.Order
        {
            Id = Guid.NewGuid(),
            OrderNum = "#1234",
            ProductId = Guid.NewGuid(),
            Product = null!,
            TotalAmount = 50.0m,
            Status = OrderStatus.NEW,
            Date = DateTime.UtcNow
        };

        // Act
        var dto = order.ToDto();

        // Assert
        Assert.Equal("Unknown Product", dto.Product.Name);
        Assert.Equal(0, dto.Product.Price);
    }
}
