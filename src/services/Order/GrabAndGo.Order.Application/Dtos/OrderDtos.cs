using GrabAndGo.Order.Domain.Enums;

namespace GrabAndGo.Order.Application.Dtos;

public record ProductDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    string? ImageUrl
);

public record OrderDto(
    Guid Id,
    string OrderNum,
    ProductDto Product,
    decimal TotalAmount,
    OrderStatus Status,
    DateTime Date
);
