using GrabAndGo.BuildingBlocks.Events;
using GrabAndGo.Catalog.Application.Consumers;
using GrabAndGo.Catalog.Domain.Entities;
using GrabAndGo.Catalog.Infrastructure.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace GrabAndGo.Catalog.UnitTests;

public class ReleaseProductStockConsumerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ILogger<ReleaseProductStockConsumer>> _loggerMock;
    private readonly ReleaseProductStockConsumer _consumer;

    public ReleaseProductStockConsumerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _loggerMock = new Mock<ILogger<ReleaseProductStockConsumer>>();
        _consumer = new ReleaseProductStockConsumer(_productRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Consume_Should_ReleaseStock_And_PublishReleasedEvent()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Quantity = 10 };

        var contextMock = new Mock<ConsumeContext<ReleaseProductStockCommand>>();
        contextMock.Setup(c => c.Message).Returns(new ReleaseProductStockCommand(orderId, productId, 2));

        _productRepositoryMock.Setup(r => r.GetProduct(productId.ToString()))
            .ReturnsAsync(product);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        Assert.Equal(12, product.Quantity);
        _productRepositoryMock.Verify(r => r.UpdateProduct(product), Times.Once);
        contextMock.Verify(c => c.Publish(It.Is<ProductStockReleasedEvent>(e => e.OrderId == orderId), default), Times.Once);
    }
}
