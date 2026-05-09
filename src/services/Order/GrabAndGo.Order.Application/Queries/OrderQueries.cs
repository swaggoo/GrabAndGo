using GrabAndGo.Order.Application.Dtos;
using GrabAndGo.Order.Domain.Enums;
using MediatR;

namespace GrabAndGo.Order.Application.Queries;

public record GetOrdersQuery(string UserId, OrderFilterStatus Filter = OrderFilterStatus.ALL) : IRequest<IEnumerable<OrderDto>>;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto?>;
