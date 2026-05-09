namespace GrabAndGo.Order.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string BusinessId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
}
