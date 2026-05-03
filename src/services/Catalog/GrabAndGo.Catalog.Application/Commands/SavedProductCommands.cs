using MediatR;

namespace GrabAndGo.Catalog.Application.Commands;

public record SaveProductCommand(string UserId, Guid ProductId) : IRequest<bool>;
public record UnsaveProductCommand(string UserId, Guid ProductId) : IRequest<bool>;
