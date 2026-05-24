using GrabAndGo.Catalog.Application.Queries;
using GrabAndGo.Catalog.Domain.Entities;
using GrabAndGo.Catalog.Infrastructure.Repositories;
using GrabAndGo.BuildingBlocks.Specifications;
using Moq;

namespace GrabAndGo.Catalog.UnitTests;

public class CatalogQueryTests
{
    private readonly Mock<IBusinessRepository> _businessRepositoryMock;

    public CatalogQueryTests()
    {
        _businessRepositoryMock = new Mock<IBusinessRepository>();
    }

    [Fact]
    public async Task GetBusinessesHandler_ShouldReturnBusinesses()
    {
        // Arrange
        var businesses = new List<Business>
        {
            new() 
            { 
                Id = Guid.NewGuid(), 
                BusinessId = "b1",
                Name = "Business 1", 
                Address = new BusinessAddress { Street = "S1", City = "C1", PostalCode = "P1", Country = "UK" },
                Location = new BusinessLocation { Latitude = 50, Longitude = 30 } 
            },
            new() 
            { 
                Id = Guid.NewGuid(), 
                BusinessId = "b2",
                Name = "Business 2", 
                Address = new BusinessAddress { Street = "S2", City = "C2", PostalCode = "P2", Country = "UK" },
                Location = new BusinessLocation { Latitude = 51, Longitude = 31 } 
            }
        };

        _businessRepositoryMock.Setup(x => x.GetBusinessesWithSpec(It.IsAny<ISpecification<Business>>()))
            .ReturnsAsync(businesses);

        var handler = new GetBusinessesHandler(_businessRepositoryMock.Object);
        var query = new GetBusinessesQuery(null, null, true, null, null, null);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetBusinessesHandler_WithLocationFiltering_ShouldReturnNearbyBusinesses()
    {
        // Arrange
        var nearBusiness = new Business 
        { 
            Id = Guid.NewGuid(), 
            BusinessId = "near",
            Name = "Near", 
            Address = new BusinessAddress { Street = "S1", City = "C1", PostalCode = "P1", Country = "UK" },
            Location = new BusinessLocation { Latitude = 50.001, Longitude = 30.001 } 
        };
        var farBusiness = new Business 
        { 
            Id = Guid.NewGuid(), 
            BusinessId = "far",
            Name = "Far", 
            Address = new BusinessAddress { Street = "S2", City = "C2", PostalCode = "P2", Country = "UK" },
            Location = new BusinessLocation { Latitude = 51, Longitude = 31 } 
        };
        
        var businesses = new List<Business> { nearBusiness, farBusiness };

        _businessRepositoryMock.Setup(x => x.GetBusinessesWithSpec(It.IsAny<ISpecification<Business>>()))
            .ReturnsAsync(businesses);

        var handler = new GetBusinessesHandler(_businessRepositoryMock.Object);
        var query = new GetBusinessesQuery(null, null, true, 50.0, 30.0, 5.0);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("Near", result.First().Name);
    }

    [Fact]
    public async Task GetAllProductsHandler_ShouldReturnProductsWithSummary()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var businessId = Guid.NewGuid().ToString();
        var categoryId = Guid.NewGuid().ToString();

        var products = new List<Product>
        {
            new() { Id = productId, Name = "Product 1", BusinessId = businessId, CategoryId = categoryId }
        };

        var business = new Business { Id = Guid.Parse(businessId), Name = "Business 1" };
        var category = new Category { Id = Guid.Parse(categoryId), Name = "Category 1" };

        var productRepoMock = new Mock<IProductRepository>();
        var categoryRepoMock = new Mock<ICategoryRepository>();

        productRepoMock.Setup(x => x.GetProducts(null, null, null, null, null, null, null, null))
            .ReturnsAsync(products);
        _businessRepositoryMock.Setup(x => x.GetBusinessById(businessId))
            .ReturnsAsync(business);
        categoryRepoMock.Setup(x => x.GetCategory(categoryId))
            .ReturnsAsync(category);

        var handler = new GetAllProductsHandler(productRepoMock.Object, _businessRepositoryMock.Object, categoryRepoMock.Object);
        var query = new GetAllProductsQuery(null, null, null, null, null, null, null, null);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Product 1", result.First().Name);
        Assert.Equal("Business 1", result.First().Business!.Name);
        Assert.Equal("Category 1", result.First().Category!.Name);
    }
}
