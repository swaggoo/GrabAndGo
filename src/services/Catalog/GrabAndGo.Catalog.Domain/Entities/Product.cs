namespace GrabAndGo.Catalog.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    
    public string BusinessId { get; set; } = default!;
    
    public string CategoryId { get; set; } = default!;
    
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    
    public decimal Price { get; set; }
    public decimal OriginalPrice { get; set; }
    
    public DateTime PickupStart { get; set; }
    public DateTime PickupEnd { get; set; }
    
    public int Quantity { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public Business Business { get; set; } = default!;
}
