using GrabAndGo.Order.Application.Dtos;
using MediatR;

namespace GrabAndGo.Order.Application.Queries;

public record GetOrdersQuery(string UserId) : IRequest<IEnumerable<OrderDto>>;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto?>;
