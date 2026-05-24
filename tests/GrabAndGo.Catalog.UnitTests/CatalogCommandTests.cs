using GrabAndGo.BuildingBlocks.Events;
using GrabAndGo.Catalog.Application.Commands;
using GrabAndGo.Catalog.Domain.Entities;
using GrabAndGo.Catalog.Infrastructure.Data;
using GrabAndGo.Catalog.Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GrabAndGo.Catalog.UnitTests;

public class CatalogCommandTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IBusinessRepository> _businessRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;

    public CatalogCommandTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _businessRepositoryMock = new Mock<IBusinessRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
    }

    [Fact]
    public async Task UpdateProductHandler_WithValidRequest_ShouldReturnTrue()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = new UpdateProductCommand(
            productId.ToString(), "Updated Name", "business-1", "category-1", "Description", "url", 12, 18, 
            DateTime.Now, DateTime.Now.AddHours(1), 10, true);

        _businessRepositoryMock.Setup(x => x.GetBusinessById(command.BusinessId))
            .ReturnsAsync(new Business { Id = Guid.NewGuid() });
        _categoryRepositoryMock.Setup(x => x.GetCategory(command.CategoryId))
            .ReturnsAsync(new Category { Id = Guid.NewGuid() });
        _productRepositoryMock.Setup(x => x.UpdateProduct(It.IsAny<Product>()))
            .ReturnsAsync(true);

        var handler = new UpdateProductHandler(
            _productRepositoryMock.Object, _businessRepositoryMock.Object, _categoryRepositoryMock.Object, _publishEndpointMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        _publishEndpointMock.Verify(x => x.Publish(It.IsAny<ProductUpdatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProductHandler_WithInvalidBusiness_ShouldThrowException()
    {
        // Arrange
        var command = new UpdateProductCommand(
            Guid.NewGuid().ToString(), "Name", "invalid-business", "category-1", null, null, 10, 15, 
            DateTime.Now, DateTime.Now, 5, true);

        _businessRepositoryMock.Setup(x => x.GetBusinessById(command.BusinessId))
            .ReturnsAsync((Business?)null);

        var handler = new UpdateProductHandler(
            _productRepositoryMock.Object, _businessRepositoryMock.Object, _categoryRepositoryMock.Object, _publishEndpointMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateProductHandler_WhenRepositoryReturnsFalse_ShouldReturnFalse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = new UpdateProductCommand(
            productId.ToString(), "Name", "business-1", "category-1", null, null, 10, 15, 
            DateTime.Now, DateTime.Now, 5, true);

        _businessRepositoryMock.Setup(x => x.GetBusinessById(command.BusinessId))
            .ReturnsAsync(new Business { Id = Guid.NewGuid() });
        _categoryRepositoryMock.Setup(x => x.GetCategory(command.CategoryId))
            .ReturnsAsync(new Category { Id = Guid.NewGuid() });
        _productRepositoryMock.Setup(x => x.UpdateProduct(It.IsAny<Product>()))
            .ReturnsAsync(false);

        var handler = new UpdateProductHandler(
            _productRepositoryMock.Object, _businessRepositoryMock.Object, _categoryRepositoryMock.Object, _publishEndpointMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        _publishEndpointMock.Verify(x => x.Publish(It.IsAny<ProductUpdatedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteProductHandler_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var productId = Guid.NewGuid().ToString();
        var command = new DeleteProductCommand(productId);

        _productRepositoryMock.Setup(x => x.DeleteProduct(productId))
            .ReturnsAsync(true);

        var handler = new DeleteProductHandler(_productRepositoryMock.Object, _publishEndpointMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        _publishEndpointMock.Verify(x => x.Publish(It.IsAny<ProductDeletedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SavedProductHandlers_SaveProduct_ShouldReturnTrue()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(databaseName: "SaveProductDb")
            .Options;

        using var context = new CatalogDbContext(options);
        var handler = new SavedProductHandlers(context);
        var productId = Guid.NewGuid();
        var command = new SaveProductCommand("user-1", productId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(1, await context.SavedProducts.CountAsync());
    }

    [Fact]
    public async Task SavedProductHandlers_UnsaveProduct_ShouldReturnTrue()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(databaseName: "UnsaveProductDb")
            .Options;

        using var context = new CatalogDbContext(options);
        var productId = Guid.NewGuid();
        context.SavedProducts.Add(new SavedProduct { UserId = "user-1", ProductId = productId });
        await context.SaveChangesAsync();

        var handler = new SavedProductHandlers(context);
        var command = new UnsaveProductCommand("user-1", productId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(0, await context.SavedProducts.CountAsync());
    }
}
