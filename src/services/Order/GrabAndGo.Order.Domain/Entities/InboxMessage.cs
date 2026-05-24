namespace GrabAndGo.Order.Domain.Entities;

public class InboxMessage
{
    public Guid Id { get; set; }
    public string ConsumerName { get; set; } = default!;
    public DateTime ProcessedOnUtc { get; set; }
}
