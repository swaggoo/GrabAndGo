using GrabAndGo.Order.Application.Dtos;
using GrabAndGo.Order.Application.Extensions;
using GrabAndGo.Order.Domain.Repositories;
using MediatR;

namespace GrabAndGo.Order.Application.Queries;

public class GetOrdersHandler(IOrderRepository orderRepository) : IRequestHandler<GetOrdersQuery, IEnumerable<OrderDto>>
{
    public async Task<IEnumerable<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await orderRepository.GetOrdersAsync(request.UserId);
        
        return orders.Select(o => o.ToDto());
    }
}

public class GetOrderByIdHandler(IOrderRepository orderRepository) : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.Id);
        return order?.ToDto();
    }
}
