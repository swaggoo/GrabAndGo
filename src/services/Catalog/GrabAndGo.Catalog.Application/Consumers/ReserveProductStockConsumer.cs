using GrabAndGo.BuildingBlocks.Events;
using GrabAndGo.Catalog.Infrastructure.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GrabAndGo.Catalog.Application.Consumers;

public class ReserveProductStockConsumer(
    IProductRepository productRepository,
    ILogger<ReserveProductStockConsumer> logger) : IConsumer<ReserveProductStockCommand>
{
    public async Task Consume(ConsumeContext<ReserveProductStockCommand> context)
    {
        var message = context.Message;
        logger.LogInformation("Attempting to reserve stock for Product: {ProductId}, Order: {OrderId}", 
            message.ProductId, message.OrderId);

        var product = await productRepository.GetProduct(message.ProductId.ToString());

        if (product == null)
        {
            logger.LogWarning("Product not found: {ProductId}", message.ProductId);
            await context.Publish(new ProductStockReservationFailedEvent(
                message.OrderId, 
                message.ProductId, 
                "Product not found"));
            return;
        }

        if (product.Quantity < message.Quantity)
        {
            logger.LogWarning("Insufficient stock for Product: {ProductId}. Requested: {Requested}, Available: {Available}", 
                message.ProductId, message.Quantity, product.Quantity);
            
            await context.Publish(new ProductStockReservationFailedEvent(
                message.OrderId, 
                message.ProductId, 
                "Insufficient stock"));
            return;
        }

        // Reserve stock
        product.Quantity -= message.Quantity;
        await productRepository.UpdateProduct(product);

        logger.LogInformation("Stock reserved for Product: {ProductId}, Order: {OrderId}", 
            message.ProductId, message.OrderId);

        await context.Publish(new ProductStockReservedEvent(
            message.OrderId, 
            message.ProductId));
    }
}
