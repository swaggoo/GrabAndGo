using GrabAndGo.Catalog.Domain.Entities;
using MediatR;

namespace GrabAndGo.Catalog.Application.Commands;

public record CreateProductCommand(
    string Name, 
    string BusinessId, 
    string CategoryId,
    string? Description, 
    string? ImageUrl, 
    decimal Price, 
    decimal OriginalPrice,
    DateTime PickupStart,
    DateTime PickupEnd,
    int Quantity) : IRequest<Product>;
