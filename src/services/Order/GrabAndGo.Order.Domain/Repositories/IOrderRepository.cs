using GrabAndGo.Order.Domain.Entities;

namespace GrabAndGo.Order.Domain.Repositories;

public interface IOrderRepository
{
    Task<IEnumerable<Entities.Order>> GetOrdersAsync(string userId);
    Task<Entities.Order?> GetByIdAsync(Guid id);
    Task<Product?> GetProductByIdAsync(Guid id);
    Task AddAsync(Entities.Order order);
    Task UpdateAsync(Entities.Order order);

    Task AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(Guid id);

    Task AddOutboxMessageAsync(OutboxMessage message);

    Task<bool> IsInboxMessageProcessedAsync(Guid messageId, string consumerName);
    Task AddInboxMessageAsync(InboxMessage message);

    Task SaveChangesAsync();
}
