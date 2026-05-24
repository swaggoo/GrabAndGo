namespace GrabAndGo.BuildingBlocks.Events;

public record OrderSubmittedEvent(
    Guid OrderId,
    Guid ProductId,
    string UserId,
    decimal TotalAmount
);

public record AcceptOrderCommand(Guid OrderId);
public record RejectOrderCommand(Guid OrderId);
