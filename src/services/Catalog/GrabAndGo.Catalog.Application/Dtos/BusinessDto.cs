namespace GrabAndGo.Catalog.Application.Dtos;

public record StoreAddressDto(
    string Street,
    string City,
    string PostalCode,
    string Country
);

public record LocationDto(
    double Latitude,
    double Longitude
);

public record BusinessDto(
    string Id,
    string Name,
    string? Description,
    string? LogoUrl,
    string? CoverImageUrl,
    StoreAddressDto Address,
    LocationDto Location,
    string? Phone,
    string? Email,
    string? Website,
    int TotalOrders,
    bool IsVerified
);
