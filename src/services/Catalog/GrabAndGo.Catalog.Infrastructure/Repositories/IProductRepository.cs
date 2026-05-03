using GrabAndGo.BuildingBlocks.Pagination;
using GrabAndGo.BuildingBlocks.Specifications;
using GrabAndGo.Catalog.Domain.Entities;

namespace GrabAndGo.Catalog.Infrastructure.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProducts(
        string? name = null, 
        string? businessId = null, 
        string? categoryId = null, 
        string? sort = null,
        bool? isActive = null,
        double? latitude = null,
        double? longitude = null,
        double? radiusInKm = null);
    
    Task<IEnumerable<Product>> GetProductsWithSpec(ISpecification<Product> spec);
    Task<PagedList<Product>> GetProductsAsync(QueryParameters queryParameters);
    
    Task<Product> GetProduct(string id);

    Task CreateProduct(Product product);
    Task<bool> UpdateProduct(Product product);
    Task<bool> DeleteProduct(string id);
}
