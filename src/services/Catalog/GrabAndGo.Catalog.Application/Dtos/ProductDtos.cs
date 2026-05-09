namespace GrabAndGo.Catalog.Application.Dtos;

public record BusinessSummaryDto(string Id, string Name, string? LogoUrl);

public record CategoryDto(string Id, string Name);

public record RatingDto(
    float OverallRating,
    int TotalRatings,
    float CollectionRating,
    float QualityRating,
    float VarietyRating,
    float QuantityRating
);

public record ProductDto(
    string Id, 
    string Name, 
    string? Description, 
    string? ImageUrl, 
    decimal Price, 
    decimal OriginalPrice, 
    DateTime PickupStart, 
    DateTime PickupEnd, 
    int Quantity, 
    RatingDto Rating,
    BusinessSummaryDto? Business,
    CategoryDto? Category,
    double? Distance = null
);
