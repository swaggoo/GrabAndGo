using GrabAndGo.Catalog.Application.Dtos;
using GrabAndGo.Catalog.Domain.Entities;

namespace GrabAndGo.Catalog.Application.Extensions;

public static class MappingExtensions
{
    public static BusinessDto ToDto(this Business business)
    {
        return new BusinessDto(
            business.BusinessId,
            business.Name,
            business.Description,
            business.LogoUrl,
            business.CoverImageUrl,
            new StoreAddressDto(
                business.Address.Street,
                business.Address.City,
                business.Address.PostalCode,
                business.Address.Country
            ),
            new LocationDto(
                business.Location.Latitude,
                business.Location.Longitude
            ),
            business.Phone,
            business.Email,
            business.Website,
            business.TotalOrders,
            business.IsVerified
        );
    }

    public static RatingDto ToDto(this Rating rating)
    {
        return new RatingDto(
            rating.OverallRating,
            rating.TotalRatings,
            rating.CollectionRating,
            rating.QualityRating,
            rating.VarietyRating,
            rating.QuantityRating
        );
    }
}
