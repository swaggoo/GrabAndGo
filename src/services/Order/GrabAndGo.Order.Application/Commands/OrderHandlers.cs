using System.Text.Json;
using GrabAndGo.Order.Application.Dtos;
using GrabAndGo.Order.Application.Extensions;
using GrabAndGo.Order.Domain.Entities;
using GrabAndGo.Order.Domain.Enums;
using GrabAndGo.Order.Domain.Repositories;
using GrabAndGo.BuildingBlocks.Events;
using MediatR;

namespace GrabAndGo.Order.Application.Commands;

public class CreateOrderHandler(IOrderRepository orderRepository) : IRequestHandler<CreateOrderCommand, OrderDto>
{
    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var product = await orderRepository.GetProductByIdAsync(request.ProductId);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {request.ProductId} not found.");
        }

        var order = new Domain.Entities.Order
        {
            Id = Guid.NewGuid(),
            OrderNum = $"#{new Random().Next(1000, 9999)}",
            UserId = request.UserId,
            BusinessId = request.BusinessId,
            ProductId = request.ProductId,
            Product = product,
            TotalAmount = request.TotalAmount,
            Status = OrderStatus.NEW,
            Date = DateTime.UtcNow
        };
        
        var @event = new OrderSubmittedEvent(
            order.Id,
            order.ProductId,
            order.UserId,
            order.TotalAmount
        );

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = typeof(OrderSubmittedEvent).FullName!,
            Content = JsonSerializer.Serialize(@event),
            OccurredOnUtc = DateTime.UtcNow
        };

        await orderRepository.AddAsync(order);
        await orderRepository.AddOutboxMessageAsync(outboxMessage);
        await orderRepository.SaveChangesAsync();
        
        return order.ToDto();
    }
}

public class UpdateOrderStatusHandler(IOrderRepository orderRepository) : IRequestHandler<UpdateOrderStatusCommand, bool>
{
    public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.OrderId);
        if (order == null) return false;

        order.Status = request.Status;
        await orderRepository.UpdateAsync(order);
        await orderRepository.SaveChangesAsync();

        return true;
    }
}
