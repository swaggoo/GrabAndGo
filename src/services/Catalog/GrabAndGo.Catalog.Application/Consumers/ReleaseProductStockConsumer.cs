using GrabAndGo.BuildingBlocks.Events;
using GrabAndGo.Catalog.Infrastructure.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GrabAndGo.Catalog.Application.Consumers;

public class ReleaseProductStockConsumer(
    IProductRepository productRepository,
    ILogger<ReleaseProductStockConsumer> logger) : IConsumer<ReleaseProductStockCommand>
{
    public async Task Consume(ConsumeContext<ReleaseProductStockCommand> context)
    {
        var message = context.Message;
        logger.LogInformation("Releasing stock for Product: {ProductId}, Order: {OrderId}", 
            message.ProductId, message.OrderId);

        var product = await productRepository.GetProduct(message.ProductId.ToString());

        if (product == null)
        {
            logger.LogError("Product not found during stock release: {ProductId}", message.ProductId);
            return;
        }

        // Return stock
        product.Quantity += message.Quantity;
        await productRepository.UpdateProduct(product);

        logger.LogInformation("Stock released for Product: {ProductId}, Order: {OrderId}. New quantity: {Quantity}", 
            message.ProductId, message.OrderId, product.Quantity);

        await context.Publish(new ProductStockReleasedEvent(
            message.OrderId, 
            message.ProductId));
    }
}
