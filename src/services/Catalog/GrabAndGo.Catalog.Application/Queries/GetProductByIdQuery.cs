using GrabAndGo.Catalog.Application.Dtos;
using MediatR;

namespace GrabAndGo.Catalog.Application.Queries;

public record GetProductByIdQuery(string Id) : IRequest<ProductDto?>;
