using GrabAndGo.Catalog.Application.Dtos;
using GrabAndGo.Catalog.Infrastructure.Data;
using GrabAndGo.Catalog.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GrabAndGo.Catalog.Application.Queries;

public class GetSavedProductsHandler(
    CatalogDbContext context,
    IBusinessRepository businessRepository,
    ICategoryRepository categoryRepository) 
    : IRequestHandler<GetSavedProductsQuery, IEnumerable<ProductDto>>
{
    public async Task<IEnumerable<ProductDto>> Handle(GetSavedProductsQuery request, CancellationToken cancellationToken)
    {
        var savedProducts = await context.SavedProducts
            .Include(x => x.Product)
            .ThenInclude(p => p.Business)
            .Where(x => x.UserId == request.UserId)
            .Select(x => x.Product)
            .ToListAsync(cancellationToken);

        var productDtos = new List<ProductDto>();
        
        var businessIds = savedProducts.Select(p => p.BusinessId).Distinct();
        var categoryIds = savedProducts.Select(p => p.CategoryId).Distinct();
        
        var businesses = new Dictionary<string, BusinessSummaryDto>();
        var categories = new Dictionary<string, CategoryDto>();
        
        foreach (var bid in businessIds)
        {
            var b = await businessRepository.GetBusinessById(bid);
            if (b != null) businesses[bid] = new BusinessSummaryDto(b.Id.ToString(), b.Name, b.LogoUrl, b.Rating);
        }

        foreach (var cid in categoryIds)
        {
            var c = await categoryRepository.GetCategory(cid);
            if (c != null) categories[cid] = new CategoryDto(c.Id.ToString(), c.Name);
        }

        foreach (var p in savedProducts)
        {
            businesses.TryGetValue(p.BusinessId, out var bSummary);
            categories.TryGetValue(p.CategoryId, out var cDto);

            productDtos.Add(new ProductDto(
                p.Id.ToString(),
                p.Name,
                p.Description,
                p.ImageUrl,
                p.Price,
                p.OriginalPrice,
                p.PickupStart,
                p.PickupEnd,
                p.Quantity,
                bSummary,
                cDto
            ));
        }

        return productDtos;
    }
}
