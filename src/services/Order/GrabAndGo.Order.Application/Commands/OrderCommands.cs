using GrabAndGo.Order.Application.Dtos;
using GrabAndGo.Order.Domain.Enums;
using MediatR;

namespace GrabAndGo.Order.Application.Commands;

public record CreateOrderCommand(
    string UserId,
    string BusinessId,
    Guid ProductId,
    decimal TotalAmount
) : IRequest<OrderDto>;

public record UpdateOrderStatusCommand(
    Guid OrderId,
    OrderStatus Status
) : IRequest<bool>;
