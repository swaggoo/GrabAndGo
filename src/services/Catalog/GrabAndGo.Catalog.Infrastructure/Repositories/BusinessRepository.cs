using GrabAndGo.BuildingBlocks.Pagination;
using GrabAndGo.BuildingBlocks.Specifications;
using GrabAndGo.Catalog.Domain.Entities;
using GrabAndGo.Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GrabAndGo.Catalog.Infrastructure.Repositories;

public class BusinessRepository : IBusinessRepository
{
    private readonly CatalogDbContext _context;

    public BusinessRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Business>> GetBusinessesWithSpec(ISpecification<Business> spec)
    {
        var query = SpecificationEvaluator<Business>.GetQuery(_context.Businesses.AsQueryable(), spec);
        return await query.ToListAsync();
    }

    public async Task<PagedList<Business>> GetBusinessesAsync(QueryParameters queryParameters)
    {
        var businesses = _context.Businesses
            .Search(queryParameters.SearchTerm)
            .ApplyFilters(queryParameters.Filters)
            .ApplySort(queryParameters.OrderBy);

        return await PagedList<Business>.ToPagedList(businesses, queryParameters.PageNumber, queryParameters.PageSize);
    }

    public async Task<IEnumerable<Business>> GetBusinesses(
        string? name = null, 
        string? sort = null,
        bool? isActive = null,
        double? latitude = null,
        double? longitude = null,
        double? radiusInKm = null)
    {
        var query = _context.Businesses.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(b => EF.Functions.Like(b.Name, $"%{name}%"));
        }

        if (isActive.HasValue)
        {
            query = query.Where(b => b.IsActive == isActive.Value);
        }

        var businesses = await query.ToListAsync();

        // Location filtering
        if (latitude.HasValue && longitude.HasValue)
        {
            var userLat = latitude.Value;
            var userLon = longitude.Value;

            // Haversine formula for distance calculation
            double GetDistance(double lat1, double lon1, double lat2, double lon2)
            {
                var r = 6371; // Earth radius in km
                var dLat = ToRadians(lat2 - lat1);
                var dLon = ToRadians(lon2 - lon1);
                var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                        Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                        Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                return r * c;
            }

            double ToRadians(double angle) => Math.PI * angle / 180.0;

            if (radiusInKm.HasValue)
            {
                businesses = businesses.Where(b => 
                    GetDistance(userLat, userLon, b.Location.Latitude, b.Location.Longitude) <= radiusInKm.Value)
                    .ToList();
            }

            if (sort == "distance")
            {
                businesses = businesses.OrderBy(b => 
                    GetDistance(userLat, userLon, b.Location.Latitude, b.Location.Longitude))
                    .ToList();
            }
        }

        if (!string.IsNullOrEmpty(sort) && sort != "distance")
        {
            businesses = sort.ToLower() switch
            {
                "name" => businesses.OrderBy(b => b.Name).ToList(),
                "name_desc" => businesses.OrderByDescending(b => b.Name).ToList(),
                _ => businesses.OrderBy(b => b.Id).ToList()
            };
        }

        return businesses;
    }

    public async Task<Business> GetBusiness(string id)
    {
        if (!Guid.TryParse(id, out var guid)) return null!;
        return await _context.Businesses.FindAsync(guid);
    }

    public async Task<Business> GetBusinessById(string businessId)
    {
        return await _context.Businesses.FirstOrDefaultAsync(b => b.BusinessId == businessId);
    }

    public async Task CreateBusiness(Business business)
    {
        _context.Businesses.Add(business);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateBusiness(Business business)
    {
        _context.Entry(business).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}
