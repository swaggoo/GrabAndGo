using GrabAndGo.Order.Application.Dtos;
using GrabAndGo.Order.Domain.Entities;

namespace GrabAndGo.Order.Application.Extensions;

public static class MappingExtensions
{
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.ImageUrl
        );
    }

    public static OrderDto ToDto(this Domain.Entities.Order order)
    {
        return new OrderDto(
            order.Id,
            order.OrderNum,
            order.Product?.ToDto() ?? new ProductDto(order.ProductId, "Unknown Product", null, 0, null),
            order.TotalAmount,
            order.Status,
            order.Date
        );
    }
}
