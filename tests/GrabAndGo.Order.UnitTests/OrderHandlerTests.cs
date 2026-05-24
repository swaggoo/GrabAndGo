using GrabAndGo.Order.Application.Commands;
using GrabAndGo.Order.Application.Queries;
using GrabAndGo.Order.Domain.Entities;
using GrabAndGo.Order.Domain.Enums;
using GrabAndGo.Order.Domain.Repositories;
using Moq;

namespace GrabAndGo.Order.UnitTests;

public class OrderHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;

    public OrderHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
    }

    [Fact]
    public async Task UpdateOrderStatusHandler_WithExistingOrder_ShouldUpdateStatusAndReturnTrue()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Domain.Entities.Order { Id = orderId, Status = OrderStatus.NEW };
        var command = new UpdateOrderStatusCommand(orderId, OrderStatus.COMPLETED);

        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        var handler = new UpdateOrderStatusHandler(_orderRepositoryMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(OrderStatus.COMPLETED, order.Status);
        _orderRepositoryMock.Verify(x => x.UpdateAsync(order), Times.Once);
        _orderRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderStatusHandler_WithNonExistingOrder_ShouldReturnFalse()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var command = new UpdateOrderStatusCommand(orderId, OrderStatus.COMPLETED);

        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync((Domain.Entities.Order?)null);

        var handler = new UpdateOrderStatusHandler(_orderRepositoryMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetOrdersHandler_ShouldReturnOrders()
    {
        // Arrange
        var userId = "user-1";
        var product = new Product { Id = Guid.NewGuid(), Name = "Test Product" };
        var orders = new List<Domain.Entities.Order>
        {
            new() { Id = Guid.NewGuid(), UserId = userId, ProductId = product.Id, Product = product },
            new() { Id = Guid.NewGuid(), UserId = userId, ProductId = product.Id, Product = product }
        };

        _orderRepositoryMock.Setup(x => x.GetOrdersAsync(userId))
            .ReturnsAsync(orders);

        var handler = new GetOrdersHandler(_orderRepositoryMock.Object);
        var query = new GetOrdersQuery(userId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetOrderByIdHandler_WithExistingOrder_ShouldReturnOrderDto()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Name = "Test Product" };
        var order = new Domain.Entities.Order 
        { 
            Id = orderId, 
            UserId = "user-1", 
            ProductId = product.Id, 
            Product = product 
        };

        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        var handler = new GetOrderByIdHandler(_orderRepositoryMock.Object);
        var query = new GetOrderByIdQuery(orderId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderId, result!.Id);
    }

    [Fact]
    public async Task GetOrderByIdHandler_WithNonExistingOrder_ShouldReturnNull()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId))
            .ReturnsAsync((Domain.Entities.Order?)null);

        var handler = new GetOrderByIdHandler(_orderRepositoryMock.Object);
        var query = new GetOrderByIdQuery(orderId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
