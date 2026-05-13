using GrabAndGo.BuildingBlocks.Events;
using GrabAndGo.Catalog.Domain.Entities;
using GrabAndGo.Catalog.Infrastructure.Repositories;
using MassTransit;
using MediatR;

namespace GrabAndGo.Catalog.Application.Commands;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateProductHandler(IProductRepository productRepository, IBusinessRepository businessRepository, ICategoryRepository categoryRepository, IPublishEndpoint publishEndpoint)
    {
        _productRepository = productRepository;
        _businessRepository = businessRepository;
        _categoryRepository = categoryRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var business = await _businessRepository.GetBusinessById(request.BusinessId);
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

        var result = await _productRepository.UpdateProduct(product);

        if (result)
        {
            await _publishEndpoint.Publish(new ProductUpdatedEvent(
                product.Id,
                product.Name,
                product.Description,
                product.Price,
                product.ImageUrl
            ), cancellationToken);
        }

        return result;
    }
}
