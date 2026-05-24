using GrabAndGo.BuildingBlocks.Events;
using GrabAndGo.Catalog.Application.Commands;
using GrabAndGo.Catalog.Application.Queries;
using GrabAndGo.Catalog.Domain.Entities;
using GrabAndGo.Catalog.Infrastructure.Repositories;
using MassTransit;
using Moq;

namespace GrabAndGo.Catalog.UnitTests;

public class ProductHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IBusinessRepository> _businessRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;

    public ProductHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _businessRepositoryMock = new Mock<IBusinessRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
    }

    [Fact]
    public async Task CreateProductHandler_WithValidRequest_ShouldCreateProductAndPublishEvent()
    {
        // Arrange
        var command = new CreateProductCommand(
            "Test Product", "business-1", "category-1", "Description", "url", 10, 15, 
            DateTime.Now, DateTime.Now.AddHours(2), 5);
        
        var business = new Business { Id = Guid.NewGuid(), Name = "Test Business" };
        var category = new Category { Id = Guid.NewGuid(), Name = "Test Category" };

        _businessRepositoryMock.Setup(x => x.GetBusinessById(command.BusinessId))
            .ReturnsAsync(business);
        
        _categoryRepositoryMock.Setup(x => x.GetCategory(command.CategoryId))
            .ReturnsAsync(category);

        var handler = new CreateProductHandler(
            _productRepositoryMock.Object, 
            _businessRepositoryMock.Object, 
            _categoryRepositoryMock.Object, 
            _publishEndpointMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Name, result.Name);
        _productRepositoryMock.Verify(x => x.CreateProduct(It.IsAny<Product>()), Times.Once);
        _publishEndpointMock.Verify(x => x.Publish(It.IsAny<ProductCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateProductHandler_WithInvalidBusiness_ShouldThrowException()
    {
        // Arrange
        var command = new CreateProductCommand(
            "Test Product", "invalid-business", "category-1", "Description", "url", 10, 15, 
            DateTime.Now, DateTime.Now.AddHours(2), 5);

        _businessRepositoryMock.Setup(x => x.GetBusinessById(command.BusinessId))
            .ReturnsAsync((Business?)null);

        var handler = new CreateProductHandler(
            _productRepositoryMock.Object, 
            _businessRepositoryMock.Object, 
            _categoryRepositoryMock.Object, 
            _publishEndpointMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task GetProductByIdHandler_WithExistingProduct_ShouldReturnProductDto()
    {
        // Arrange
        var productId = Guid.NewGuid().ToString();
        var businessId = Guid.NewGuid().ToString();
        var categoryId = Guid.NewGuid().ToString();
        
        var product = new Product 
        { 
            Id = Guid.Parse(productId), 
            Name = "Test Product", 
            BusinessId = businessId, 
            CategoryId = categoryId,
            Price = 10
        };
        var business = new Business { Id = Guid.Parse(businessId), Name = "Test Business" };
        var category = new Category { Id = Guid.Parse(categoryId), Name = "Test Category" };

        _productRepositoryMock.Setup(x => x.GetProduct(productId))
            .ReturnsAsync(product);
        _businessRepositoryMock.Setup(x => x.GetBusinessById(businessId))
            .ReturnsAsync(business);
        _categoryRepositoryMock.Setup(x => x.GetCategory(categoryId))
            .ReturnsAsync(category);

        var handler = new GetProductByIdHandler(
            _productRepositoryMock.Object, 
            _categoryRepositoryMock.Object, 
            _businessRepositoryMock.Object);

        var query = new GetProductByIdQuery(productId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Name, result!.Name);
        Assert.Equal(business.Name, result.Business!.Name);
        Assert.Equal(category.Name, result.Category!.Name);
    }

    [Fact]
    public async Task GetProductByIdHandler_WithNonExistingProduct_ShouldReturnNull()
    {
        // Arrange
        var productId = Guid.NewGuid().ToString();
        _productRepositoryMock.Setup(x => x.GetProduct(productId))
            .ReturnsAsync((Product?)null);

        var handler = new GetProductByIdHandler(
            _productRepositoryMock.Object, 
            _categoryRepositoryMock.Object, 
            _businessRepositoryMock.Object);

        var query = new GetProductByIdQuery(productId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
