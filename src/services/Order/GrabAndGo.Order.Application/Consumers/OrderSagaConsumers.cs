using GrabAndGo.BuildingBlocks.Events;
using GrabAndGo.Order.Domain.Entities;
using GrabAndGo.Order.Domain.Enums;
using GrabAndGo.Order.Domain.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GrabAndGo.Order.Application.Consumers;

public class AcceptOrderConsumer(
    IOrderRepository orderRepository,
    ILogger<AcceptOrderConsumer> logger) : IConsumer<AcceptOrderCommand>
{
    public async Task Consume(ConsumeContext<AcceptOrderCommand> context)
    {
        var messageId = context.MessageId ?? context.Message.OrderId; // Fallback to OrderId if MessageId is null
        var consumerName = nameof(AcceptOrderConsumer);

        if (await orderRepository.IsInboxMessageProcessedAsync(messageId, consumerName))
        {
            logger.LogInformation("Message {MessageId} already processed by {ConsumerName}", messageId, consumerName);
            return;
        }

        logger.LogInformation("Accepting order: {OrderId}", context.Message.OrderId);
        
        var order = await orderRepository.GetByIdAsync(context.Message.OrderId);
        if (order == null)
        {
            logger.LogError("Order not found: {OrderId}", context.Message.OrderId);
            return;
        }

        order.Status = OrderStatus.ACCEPTED;
        
        await orderRepository.AddInboxMessageAsync(new InboxMessage
        {
            Id = messageId,
            ConsumerName = consumerName,
            ProcessedOnUtc = DateTime.UtcNow
        });

        await orderRepository.UpdateAsync(order);
        await orderRepository.SaveChangesAsync();
    }
}

public class RejectOrderConsumer(
    IOrderRepository orderRepository,
    ILogger<RejectOrderConsumer> logger) : IConsumer<RejectOrderCommand>
{
    public async Task Consume(ConsumeContext<RejectOrderCommand> context)
    {
        var messageId = context.MessageId ?? context.Message.OrderId;
        var consumerName = nameof(RejectOrderConsumer);

        if (await orderRepository.IsInboxMessageProcessedAsync(messageId, consumerName))
        {
            logger.LogInformation("Message {MessageId} already processed by {ConsumerName}", messageId, consumerName);
            return;
        }

        logger.LogInformation("Rejecting order: {OrderId}", context.Message.OrderId);
        
        var order = await orderRepository.GetByIdAsync(context.Message.OrderId);
        if (order == null)
        {
            logger.LogError("Order not found: {OrderId}", context.Message.OrderId);
            return;
        }

        order.Status = OrderStatus.REJECTED;

        await orderRepository.AddInboxMessageAsync(new InboxMessage
        {
            Id = messageId,
            ConsumerName = consumerName,
            ProcessedOnUtc = DateTime.UtcNow
        });

        await orderRepository.UpdateAsync(order);
        await orderRepository.SaveChangesAsync();
    }
}
