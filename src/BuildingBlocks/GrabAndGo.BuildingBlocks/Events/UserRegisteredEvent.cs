namespace GrabAndGo.BuildingBlocks.Events;

public record UserRegisteredEvent
{
    public string UserId { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Role { get; init; } = default!;
}
