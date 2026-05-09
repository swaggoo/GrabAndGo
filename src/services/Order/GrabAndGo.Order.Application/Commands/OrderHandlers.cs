using GrabAndGo.Order.Application.Dtos;
using GrabAndGo.Order.Application.Extensions;
using GrabAndGo.Order.Domain.Entities;
using GrabAndGo.Order.Domain.Enums;
using GrabAndGo.Order.Domain.Repositories;
using MediatR;

namespace GrabAndGo.Order.Application.Commands;

public class CreateOrderHandler(IOrderRepository orderRepository) : IRequestHandler<CreateOrderCommand, OrderDto>
{
    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Domain.Entities.Order
        {
            Id = Guid.NewGuid(),
            OrderNum = $"#{new Random().Next(1000, 9999)}",
            UserId = request.UserId,
            BusinessId = request.BusinessId,
            ProductId = request.ProductId,
            TotalAmount = request.TotalAmount,
            Status = OrderStatus.NEW,
            Date = DateTime.UtcNow
        };

        // In a real scenario, we'd fetch the product details from Catalog service or local replica
        // For now, we assume it's attached or handled by the repository
        
        await orderRepository.AddAsync(order);
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
