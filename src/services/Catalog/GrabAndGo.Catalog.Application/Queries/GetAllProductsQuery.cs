using GrabAndGo.Catalog.Application.Dtos;
using MediatR;

namespace GrabAndGo.Catalog.Application.Queries;

public record GetAllProductsQuery(
    string? BusinessId = null, 
    string? CategoryId = null, 
    string? Name = null, 
    string? Sort = null,
    bool? IsActive = null,
    double? Latitude = null,
    double? Longitude = null,
    double? RadiusInKm = null
) : IRequest<IEnumerable<ProductDto>>;
