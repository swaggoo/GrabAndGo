namespace GrabAndGo.BuildingBlocks.Events;

public record BusinessOnboardedEvent
{
    public string BusinessId { get; init; } = default!;
    public string BusinessName { get; init; } = default!;
    public string? Description { get; init; }
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string Email { get; init; } = default!;
    public string Category { get; init; } = default!;
    public int TotalOrders { get; init; }
    
    // Address
    public string Street { get; init; } = default!;
    public string City { get; init; } = default!;
    public string PostalCode { get; init; } = default!;
    public string Country { get; init; } = default!;
    
    // Location
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}
