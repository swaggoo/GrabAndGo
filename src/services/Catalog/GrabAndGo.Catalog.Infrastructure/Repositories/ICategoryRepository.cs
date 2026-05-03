using GrabAndGo.Catalog.Domain.Entities;

namespace GrabAndGo.Catalog.Infrastructure.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetCategories();
    Task<Category> GetCategory(string id);
}
