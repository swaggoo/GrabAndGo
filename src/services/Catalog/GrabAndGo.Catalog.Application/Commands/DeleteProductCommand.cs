using MediatR;

namespace GrabAndGo.Catalog.Application.Commands;

public record DeleteProductCommand(string Id) : IRequest<bool>;
