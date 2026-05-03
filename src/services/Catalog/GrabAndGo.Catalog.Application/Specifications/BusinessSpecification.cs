using GrabAndGo.BuildingBlocks.Specifications;
using GrabAndGo.Catalog.Domain.Entities;

namespace GrabAndGo.Catalog.Application.Specifications;

public class BusinessSpecification : BaseSpecification<Business>
{
    public double? Latitude { get; }
    public double? Longitude { get; }
    public double? RadiusInKm { get; }

    public BusinessSpecification(string? name, bool? isActive, double? latitude = null, double? longitude = null, double? radiusInKm = null) 
        : base(x => 
            (string.IsNullOrEmpty(name) || x.Name.Contains(name)) &&
            (!isActive.HasValue || x.IsActive == isActive))
    {
        Latitude = latitude;
        Longitude = longitude;
        RadiusInKm = radiusInKm;
    }

    public void ApplySorting(string? sort)
    {
        if (string.IsNullOrEmpty(sort)) return;

        switch (sort.ToLower())
        {
            case "name":
                AddOrderBy(x => x.Name);
                break;
            case "name_desc":
                AddOrderByDescending(x => x.Name);
                break;
        }
    }
}
