using GrabAndGo.Order.Application.Commands;
using GrabAndGo.Order.Domain.Entities;
using GrabAndGo.Order.Domain.Repositories;
using Moq;

namespace GrabAndGo.Order.UnitTests;

public class CreateOrderHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly CreateOrderHandler _handler;

    public CreateOrderHandlerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _handler = new CreateOrderHandler(_orderRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_CreateOrder_And_AddOutboxMessage()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "Test Product", Price = 10.0m };
        
        _orderRepositoryMock.Setup(r => r.GetProductByIdAsync(productId))
            .ReturnsAsync(product);

        var command = new CreateOrderCommand("user-1", "biz-1", productId, 10.0m);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _orderRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Order>()), Times.Once);
        _orderRepositoryMock.Verify(r => r.AddOutboxMessageAsync(It.IsAny<OutboxMessage>()), Times.Once);
        _orderRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        
        _orderRepositoryMock.Verify(r => r.AddOutboxMessageAsync(It.Is<OutboxMessage>(m => 
            m.Type.Contains("OrderSubmittedEvent"))), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowException_When_ProductNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _orderRepositoryMock.Setup(r => r.GetProductByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        var command = new CreateOrderCommand("user-1", "biz-1", productId, 10.0m);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
