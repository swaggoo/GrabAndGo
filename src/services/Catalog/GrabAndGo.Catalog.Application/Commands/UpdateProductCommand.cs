using MediatR;

namespace GrabAndGo.Catalog.Application.Commands;

public record UpdateProductCommand(
    string Id, 
    string Name, 
    string BusinessId, 
    string CategoryId,
    string? Description, 
    string? ImageUrl, 
    decimal Price, 
    decimal OriginalPrice,
    DateTime PickupStart,
    DateTime PickupEnd,
    int Quantity,
    bool IsActive) : IRequest<bool>;
