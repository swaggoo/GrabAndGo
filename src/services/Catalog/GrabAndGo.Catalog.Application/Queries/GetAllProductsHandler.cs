using GrabAndGo.Catalog.Application.Dtos;
using GrabAndGo.Catalog.Infrastructure.Repositories;
using MediatR;

namespace GrabAndGo.Catalog.Application.Queries;

public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly ICategoryRepository _categoryRepository;

    public GetAllProductsHandler(IProductRepository productRepository, IBusinessRepository businessRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _businessRepository = businessRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetProducts(
            request.Name, 
            request.BusinessId, 
            request.CategoryId, 
            request.Sort,
            request.IsActive,
            request.Latitude,
            request.Longitude,
            request.RadiusInKm);
        
        var productDtos = new List<ProductDto>();
        
        var businessIdsFromProducts = products
            .Select(p => p.BusinessId)
            .Where(id => !string.IsNullOrEmpty(id))
            .Distinct();
            
        var categoryIds = products
            .Select(p => p.CategoryId)
            .Where(id => !string.IsNullOrEmpty(id))
            .Distinct();
            
        var businesses = new Dictionary<string, BusinessSummaryDto>();
        var categories = new Dictionary<string, CategoryDto>();
        
        foreach (var businessId in businessIdsFromProducts)
        {
            var business = await _businessRepository.GetBusinessById(businessId!);
            if (business != null)
            {
                businesses[businessId!] = new BusinessSummaryDto(business.Id.ToString(), business.Name, business.LogoUrl, business.Rating);
            }
        }

        foreach (var categoryId in categoryIds)
        {
            var category = await _categoryRepository.GetCategory(categoryId!);
            if (category != null)
            {
                categories[categoryId!] = new CategoryDto(category.Id.ToString(), category.Name);
            }
        }

        foreach (var product in products)
        {
            BusinessSummaryDto? businessSummary = null;
            if (!string.IsNullOrEmpty(product.BusinessId))
            {
                businesses.TryGetValue(product.BusinessId, out businessSummary);
            }

            CategoryDto? categoryDto = null;
            if (!string.IsNullOrEmpty(product.CategoryId))
            {
                categories.TryGetValue(product.CategoryId, out categoryDto);
            }

            double? distance = null;
            if (request.Latitude.HasValue && request.Longitude.HasValue && product.Business?.Location != null)
            {
                distance = CalculateDistance(
                    request.Latitude.Value, 
                    request.Longitude.Value, 
                    product.Business.Location.Latitude, 
                    product.Business.Location.Longitude);
            }

            productDtos.Add(new ProductDto(
                product.Id.ToString(),
                product.Name,
                product.Description,
                product.ImageUrl,
                product.Price,
                product.OriginalPrice,
                product.PickupStart,
                product.PickupEnd,
                product.Quantity,
                businessSummary,
                categoryDto,
                distance
            ));
        }

        return productDtos;
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
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

    private double ToRadians(double angle) => Math.PI * angle / 180.0;
}
