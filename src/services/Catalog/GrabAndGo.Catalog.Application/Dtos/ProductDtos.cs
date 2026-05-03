namespace GrabAndGo.Catalog.Application.Dtos;

public record BusinessSummaryDto(string Id, string Name, string? LogoUrl, float? Rating);

public record CategoryDto(string Id, string Name);

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
    BusinessSummaryDto? Business,
    CategoryDto? Category,
    double? Distance = null
);
