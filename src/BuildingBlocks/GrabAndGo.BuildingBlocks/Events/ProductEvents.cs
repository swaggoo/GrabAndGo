namespace GrabAndGo.BuildingBlocks.Events;

public record ProductCreatedEvent(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    string? ImageUrl,
    string BusinessId
);

public record ProductUpdatedEvent(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    string? ImageUrl
);

public record ProductDeletedEvent(Guid Id);
