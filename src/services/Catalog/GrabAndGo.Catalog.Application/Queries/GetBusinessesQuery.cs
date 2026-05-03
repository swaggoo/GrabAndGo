using GrabAndGo.Catalog.Application.Dtos;
using MediatR;

namespace GrabAndGo.Catalog.Application.Queries;

public record GetBusinessesQuery(
    string? Name = null, 
    string? Sort = null, 
    bool? IsActive = null,
    double? Latitude = null,
    double? Longitude = null,
    double? RadiusInKm = null
) : IRequest<IEnumerable<BusinessDto>>;
