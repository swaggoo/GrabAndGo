using GrabAndGo.BuildingBlocks.Events;
using GrabAndGo.Catalog.Application.Consumers;
using GrabAndGo.Catalog.Domain.Entities;
using GrabAndGo.Catalog.Infrastructure.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace GrabAndGo.Catalog.UnitTests;

public class ReserveProductStockConsumerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ILogger<ReserveProductStockConsumer>> _loggerMock;
    private readonly ReserveProductStockConsumer _consumer;

    public ReserveProductStockConsumerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _loggerMock = new Mock<ILogger<ReserveProductStockConsumer>>();
        _consumer = new ReserveProductStockConsumer(_productRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Consume_Should_ReserveStock_And_PublishSuccess_When_StockAvailable()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Quantity = 5 };

        var contextMock = new Mock<ConsumeContext<ReserveProductStockCommand>>();
        contextMock.Setup(c => c.Message).Returns(new ReserveProductStockCommand(orderId, productId, 1));

        _productRepositoryMock.Setup(r => r.GetProduct(productId.ToString()))
            .ReturnsAsync(product);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        Assert.Equal(4, product.Quantity);
        _productRepositoryMock.Verify(r => r.UpdateProduct(product), Times.Once);
        contextMock.Verify(c => c.Publish(It.IsAny<ProductStockReservedEvent>(), default), Times.Once);
    }

    [Fact]
    public async Task Consume_Should_PublishFailure_When_StockInsufficient()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Quantity = 0 };

        var contextMock = new Mock<ConsumeContext<ReserveProductStockCommand>>();
        contextMock.Setup(c => c.Message).Returns(new ReserveProductStockCommand(orderId, productId, 1));

        _productRepositoryMock.Setup(r => r.GetProduct(productId.ToString()))
            .ReturnsAsync(product);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        Assert.Equal(0, product.Quantity); // No change
        _productRepositoryMock.Verify(r => r.UpdateProduct(It.IsAny<Product>()), Times.Never);
        contextMock.Verify(c => c.Publish(It.Is<ProductStockReservationFailedEvent>(e => e.Reason == "Insufficient stock"), default), Times.Once);
    }
}
