using GrabAndGo.BuildingBlocks.Events;
using GrabAndGo.Order.Domain.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GrabAndGo.Order.Application.Consumers;

public class ProductUpdatedConsumer(
    IOrderRepository orderRepository,
    ILogger<ProductUpdatedConsumer> logger) : IConsumer<ProductUpdatedEvent>
{
    public async Task Consume(ConsumeContext<ProductUpdatedEvent> context)
    {
        var @event = context.Message;
        logger.LogInformation("Consuming ProductUpdatedEvent: {ProductId}", @event.Id);

        var product = await orderRepository.GetProductByIdAsync(@event.Id);
        if (product == null)
        {
            logger.LogWarning("Product with ID {ProductId} not found in Order database. Cannot update.", @event.Id);
            return;
        }

        product.Name = @event.Name;
        product.Description = @event.Description;
        product.Price = @event.Price;
        product.ImageUrl = @event.ImageUrl;

        await orderRepository.UpdateProductAsync(product);
        await orderRepository.SaveChangesAsync();
    }
}
