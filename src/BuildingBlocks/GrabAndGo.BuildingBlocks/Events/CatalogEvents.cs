namespace GrabAndGo.BuildingBlocks.Events;

public record ReserveProductStockCommand(
    Guid OrderId,
    Guid ProductId,
    int Quantity
);

public record ProductStockReservedEvent(
    Guid OrderId,
    Guid ProductId
);

public record ProductStockReservationFailedEvent(
    Guid OrderId,
    Guid ProductId,
    string Reason
);

public record ReleaseProductStockCommand(
    Guid OrderId,
    Guid ProductId,
    int Quantity
);

public record ProductStockReleasedEvent(
    Guid OrderId,
    Guid ProductId
);
