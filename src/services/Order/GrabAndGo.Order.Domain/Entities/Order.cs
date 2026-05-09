using GrabAndGo.Order.Domain.Enums;

namespace GrabAndGo.Order.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public string OrderNum { get; set; } = default!;
    
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;
    
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime Date { get; set; }
    
    // Additional useful properties for a real system
    public string UserId { get; set; } = default!;
    public string BusinessId { get; set; } = default!;
}
