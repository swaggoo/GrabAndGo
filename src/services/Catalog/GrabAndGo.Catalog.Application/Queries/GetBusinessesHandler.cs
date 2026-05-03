using GrabAndGo.Catalog.Application.Dtos;
using GrabAndGo.Catalog.Application.Extensions;
using GrabAndGo.Catalog.Application.Specifications;
using GrabAndGo.Catalog.Infrastructure.Repositories;
using MediatR;

namespace GrabAndGo.Catalog.Application.Queries;

public class GetBusinessesHandler(IBusinessRepository businessRepository) : IRequestHandler<GetBusinessesQuery, IEnumerable<BusinessDto>>
{
    public async Task<IEnumerable<BusinessDto>> Handle(GetBusinessesQuery request, CancellationToken cancellationToken)
    {
        var spec = new BusinessSpecification(
            request.Name, 
            request.IsActive, 
            request.Latitude, 
            request.Longitude, 
            request.RadiusInKm);
            
        spec.ApplySorting(request.Sort);
        
        var businesses = await businessRepository.GetBusinessesWithSpec(spec);
        
        // Apply location filtering in memory
        if (request.Latitude.HasValue && request.Longitude.HasValue)
        {
            if (request.RadiusInKm.HasValue)
            {
                businesses = businesses.Where(b => 
                    CalculateDistance(request.Latitude.Value, request.Longitude.Value, b.Location.Latitude, b.Location.Longitude) <= request.RadiusInKm.Value);
            }

            // Sorting by distance in-memory if requested
            if (request.Sort == "distance")
            {
                businesses = businesses.OrderBy(b => CalculateDistance(
                    request.Latitude.Value, 
                    request.Longitude.Value, 
                    b.Location.Latitude, 
                    b.Location.Longitude));
            }
        }
        
        return businesses.Select(b => b.ToDto());
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var r = 6371; 
        var dLat = (lat2 - lat1) * Math.PI / 180.0;
        var dLon = (lon2 - lon1) * Math.PI / 180.0;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return r * c;
    }
}
