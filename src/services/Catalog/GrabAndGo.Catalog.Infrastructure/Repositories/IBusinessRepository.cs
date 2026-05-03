using GrabAndGo.BuildingBlocks.Pagination;
using GrabAndGo.BuildingBlocks.Specifications;
using GrabAndGo.Catalog.Domain.Entities;

namespace GrabAndGo.Catalog.Infrastructure.Repositories;

public interface IBusinessRepository
{
    Task<IEnumerable<Business>> GetBusinesses(
        string? name = null, 
        string? sort = null,
        bool? isActive = null,
        double? latitude = null,
        double? longitude = null,
        double? radiusInKm = null);
    Task<IEnumerable<Business>> GetBusinessesWithSpec(ISpecification<Business> spec);
    Task<PagedList<Business>> GetBusinessesAsync(QueryParameters queryParameters);
    
    Task<Business> GetBusiness(string id);
    Task<Business> GetBusinessById(string businessId);
    Task CreateBusiness(Business business);
    Task UpdateBusiness(Business business);
}
