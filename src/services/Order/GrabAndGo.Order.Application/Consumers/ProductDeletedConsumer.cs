using GrabAndGo.BuildingBlocks.Events;
using GrabAndGo.Order.Domain.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GrabAndGo.Order.Application.Consumers;

public class ProductDeletedConsumer(
    IOrderRepository orderRepository,
    ILogger<ProductDeletedConsumer> logger) : IConsumer<ProductDeletedEvent>
{
    public async Task Consume(ConsumeContext<ProductDeletedEvent> context)
    {
        var @event = context.Message;
        logger.LogInformation("Consuming ProductDeletedEvent: {ProductId}", @event.Id);

        await orderRepository.DeleteProductAsync(@event.Id);
        await orderRepository.SaveChangesAsync();
    }
}
