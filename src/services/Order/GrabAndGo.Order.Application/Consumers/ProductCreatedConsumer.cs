using GrabAndGo.BuildingBlocks.Events;
using GrabAndGo.Order.Domain.Entities;
using GrabAndGo.Order.Domain.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GrabAndGo.Order.Application.Consumers;

public class ProductCreatedConsumer(
    IOrderRepository orderRepository,
    ILogger<ProductCreatedConsumer> logger) : IConsumer<ProductCreatedEvent>
{
    public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        var @event = context.Message;
        logger.LogInformation("Consuming ProductCreatedEvent: {ProductId}", @event.Id);

        var existingProduct = await orderRepository.GetProductByIdAsync(@event.Id);
        if (existingProduct != null)
        {
            logger.LogWarning("Product with ID {ProductId} already exists in Order database.", @event.Id);
            return;
        }

        var product = new Product
        {
            Id = @event.Id,
            Name = @event.Name,
            Description = @event.Description,
            Price = @event.Price,
            ImageUrl = @event.ImageUrl,
            BusinessId = @event.BusinessId
        };

        await orderRepository.AddProductAsync(product);
        await orderRepository.SaveChangesAsync();
    }
}
