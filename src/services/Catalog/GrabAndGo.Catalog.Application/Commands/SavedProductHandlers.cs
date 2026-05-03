using GrabAndGo.Catalog.Domain.Entities;
using GrabAndGo.Catalog.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GrabAndGo.Catalog.Application.Commands;

public class SavedProductHandlers(CatalogDbContext context) 
    : IRequestHandler<SaveProductCommand, bool>,
      IRequestHandler<UnsaveProductCommand, bool>
{
    public async Task<bool> Handle(SaveProductCommand request, CancellationToken cancellationToken)
    {
        var alreadySaved = await context.SavedProducts
            .AnyAsync(x => x.UserId == request.UserId && x.ProductId == request.ProductId, cancellationToken);

        if (alreadySaved) return true;

        var savedProduct = new SavedProduct
        {
            UserId = request.UserId,
            ProductId = request.ProductId
        };

        context.SavedProducts.Add(savedProduct);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> Handle(UnsaveProductCommand request, CancellationToken cancellationToken)
    {
        var savedProduct = await context.SavedProducts
            .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.ProductId == request.ProductId, cancellationToken);

        if (savedProduct == null) return true;

        context.SavedProducts.Remove(savedProduct);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
