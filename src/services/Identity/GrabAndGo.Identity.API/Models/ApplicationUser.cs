using Microsoft.AspNetCore.Identity;

namespace GrabAndGo.Identity.API.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? LogoUrl { get; set; }
    
    // Merchant Profile Fields
    public string? BusinessName { get; set; }
    public string? BusinessDescription { get; set; }
    public string? BusinessCategory { get; set; }
    public string? BusinessPhone { get; set; }
    public string? BusinessWebsite { get; set; }
    public string? CoverImageUrl { get; set; }
    public int TotalOrders { get; set; } = 0;
    
    // Address
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    
    // Location
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
