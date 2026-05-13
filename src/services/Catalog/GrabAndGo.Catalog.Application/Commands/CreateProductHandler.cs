using GrabAndGo.BuildingBlocks.Events;
using GrabAndGo.Catalog.Domain.Entities;
using GrabAndGo.Catalog.Infrastructure.Repositories;
using MassTransit;
using MediatR;

namespace GrabAndGo.Catalog.Application.Commands;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Product>
{
    private readonly IProductRepository _productRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateProductHandler(IProductRepository productRepository, IBusinessRepository businessRepository, ICategoryRepository categoryRepository, IPublishEndpoint publishEndpoint)
    {
        _productRepository = productRepository;
        _businessRepository = businessRepository;
        _categoryRepository = categoryRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
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

        await _publishEndpoint.Publish(new ProductCreatedEvent(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.ImageUrl,
            product.BusinessId
        ), cancellationToken);

        return product;
    }
}
