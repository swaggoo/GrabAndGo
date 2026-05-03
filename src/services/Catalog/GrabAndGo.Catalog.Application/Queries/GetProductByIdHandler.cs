using GrabAndGo.Catalog.Application.Dtos;
using GrabAndGo.Catalog.Infrastructure.Repositories;
using MediatR;

namespace GrabAndGo.Catalog.Application.Queries;

public class GetProductByIdHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IBusinessRepository businessRepository)
    : IRequestHandler<GetProductByIdQuery, ProductDto?>
{

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetProduct(request.Id);
        if (product == null) return null;

        var business = await businessRepository.GetBusinessById(product.BusinessId);
        var businessSummary = business != null 
            ? new BusinessSummaryDto(business.Id.ToString(), business.Name, business.LogoUrl, business.Rating)
            : null;

        var category = !string.IsNullOrEmpty(product.CategoryId) 
            ? await categoryRepository.GetCategory(product.CategoryId)
            : null;
        var categoryDto = category != null
            ? new CategoryDto(category.Id.ToString(), category.Name)
            : null;

        return new ProductDto(
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
            categoryDto
        );
    }
}
