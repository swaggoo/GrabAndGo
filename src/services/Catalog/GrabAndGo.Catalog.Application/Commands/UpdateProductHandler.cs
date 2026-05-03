using GrabAndGo.Catalog.Domain.Entities;
using GrabAndGo.Catalog.Infrastructure.Repositories;
using MediatR;

namespace GrabAndGo.Catalog.Application.Commands;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly ICategoryRepository _categoryRepository;

    public UpdateProductHandler(IProductRepository productRepository, IBusinessRepository businessRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _businessRepository = businessRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
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
            Id = Guid.Parse(request.Id),
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
            IsActive = request.IsActive
        };

        return await _productRepository.UpdateProduct(product);
    }
}
