namespace GrabAndGo.Catalog.Domain.Entities;

public class Business
{
    public Guid Id { get; set; }
    
    // Link to Identity User
    public string BusinessId { get; set; } = default!;
    
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    
    public BusinessAddress Address { get; set; } = default!;
    public BusinessLocation Location { get; set; } = default!;
    
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    
    [System.ComponentModel.DataAnnotations.Schema.Column("TotalOrdersSaved")]
    public int TotalOrders { get; set; }
    
    public bool IsVerified { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class BusinessAddress
{
    public string Street { get; set; } = default!;
    public string City { get; set; } = default!;
    public string PostalCode { get; set; } = default!;
    public string Country { get; set; } = default!;
}

public class BusinessLocation
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
