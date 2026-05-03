using GrabAndGo.Catalog.Domain.Entities;
using GrabAndGo.Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GrabAndGo.Catalog.Infrastructure.Repositories;

public class CategoryRepository(CatalogDbContext context) : ICategoryRepository
{
    private readonly CatalogDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<IEnumerable<Category>> GetCategories()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<Category> GetCategory(string id)
    {
        if (!Guid.TryParse(id, out var guid)) return null!;
        return await _context.Categories.FindAsync(guid);
    }
}
