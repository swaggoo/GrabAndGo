using GrabAndGo.Order.Domain.Entities;

namespace GrabAndGo.Order.Domain.Repositories;

public interface IOrderRepository
{
    Task<IEnumerable<Entities.Order>> GetOrdersAsync(string userId);
    Task<Entities.Order?> GetByIdAsync(Guid id);
    Task AddAsync(Entities.Order order);
    Task UpdateAsync(Entities.Order order);
    Task SaveChangesAsync();
}
