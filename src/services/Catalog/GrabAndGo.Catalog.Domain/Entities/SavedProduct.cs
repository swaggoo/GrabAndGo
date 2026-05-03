namespace GrabAndGo.Catalog.Domain.Entities;

public class SavedProduct
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = default!;
    public Guid ProductId { get; set; }
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Product Product { get; set; } = default!;
}
