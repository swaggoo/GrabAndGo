using GrabAndGo.BuildingBlocks.Pagination;
using GrabAndGo.BuildingBlocks.Specifications;
using GrabAndGo.Catalog.Domain.Entities;
using GrabAndGo.Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GrabAndGo.Catalog.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly CatalogDbContext _context;

    public ProductRepository(CatalogDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Product>> GetProductsWithSpec(ISpecification<Product> spec)
    {
        return await SpecificationEvaluator<Product>.GetQuery(_context.Products.AsQueryable(), spec).ToListAsync();
    }

    public async Task<PagedList<Product>> GetProductsAsync(QueryParameters queryParameters)
    {
        var products = _context.Products
            .Search(queryParameters.SearchTerm)
            .ApplyFilters(queryParameters.Filters)
            .ApplySort(queryParameters.OrderBy);

        return await PagedList<Product>.ToPagedList(products, queryParameters.PageNumber, queryParameters.PageSize);
    }

    public async Task<IEnumerable<Product>> GetProducts(
        string? name = null, 
        string? businessId = null, 
        string? categoryId = null, 
        string? sort = null,
        bool? isActive = null,
        double? latitude = null,
        double? longitude = null,
        double? radiusInKm = null)
    {
        var query = _context.Products
            .Include(p => p.Business)
            .AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(p => EF.Functions.Like(p.Name, $"%{name}%"));
        }

        if (!string.IsNullOrEmpty(businessId))
        {
            query = query.Where(p => p.BusinessId == businessId);
        }

        if (!string.IsNullOrEmpty(categoryId))
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        var products = await query.ToListAsync();

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
                products = products.Where(p => 
                    GetDistance(userLat, userLon, p.Business.Location.Latitude, p.Business.Location.Longitude) <= radiusInKm.Value)
                    .ToList();
            }

            if (sort == "distance")
            {
                products = products.OrderBy(p => 
                    GetDistance(userLat, userLon, p.Business.Location.Latitude, p.Business.Location.Longitude))
                    .ToList();
            }
        }

        if (!string.IsNullOrEmpty(sort) && sort != "distance")
        {
            products = sort.ToLower() switch
            {
                "name" => products.OrderBy(p => p.Name).ToList(),
                "-name" => products.OrderByDescending(p => p.Name).ToList(),
                "price" => products.OrderBy(p => p.Price).ToList(),
                "-price" => products.OrderByDescending(p => p.Price).ToList(),
                _ => products
            };
        }

        return products;
    }

    public async Task<Product> GetProduct(string id)
    {
        if (!Guid.TryParse(id, out var guid)) return null!;
        return await _context.Products.FindAsync(guid);
    }

    public async Task CreateProduct(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateProduct(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteProduct(string id)
    {
        if (!Guid.TryParse(id, out var guid)) return false;
        var product = await _context.Products.FindAsync(guid);
        if (product == null) return false;

        _context.Products.Remove(product);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
}
