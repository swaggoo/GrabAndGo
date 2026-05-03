using GrabAndGo.Catalog.Domain.Entities;
using GrabAndGo.Catalog.Infrastructure.Repositories;
using MediatR;

namespace GrabAndGo.Catalog.Application.Commands;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Product>
{
    private readonly IProductRepository _productRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateProductHandler(IProductRepository productRepository, IBusinessRepository businessRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _businessRepository = businessRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var business = await _businessRepository.GetBusiness(request.BusinessId);
        if (business == null)
        {
            throw new Exception("Business not found");
        }

        var category = await _categoryRepository.GetCategory(request.CategoryId);
        if (category == null)
        {
            throw new Exception("Category not found");
        }

        var product = new Product
        {
            Name = request.Name,
            BusinessId = request.BusinessId,
            CategoryId = request.CategoryId,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            Price = request.Price,
            OriginalPrice = request.OriginalPrice,
            PickupStart = request.PickupStart,
            PickupEnd = request.PickupEnd,
            Quantity = request.Quantity,
            IsActive = true
        };

        await _productRepository.CreateProduct(product);
        return product;
    }
}
