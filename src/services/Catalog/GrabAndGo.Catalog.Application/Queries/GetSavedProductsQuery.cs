using GrabAndGo.Catalog.Application.Dtos;
using MediatR;

namespace GrabAndGo.Catalog.Application.Queries;

public record GetSavedProductsQuery(string UserId) : IRequest<IEnumerable<ProductDto>>;
