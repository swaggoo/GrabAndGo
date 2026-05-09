using GrabAndGo.Order.Domain.Entities;
using GrabAndGo.Order.Domain.Repositories;
using GrabAndGo.Order.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GrabAndGo.Order.Infrastructure.Repositories;

public class OrderRepository(OrderDbContext context) : IOrderRepository
{
    public async Task<IEnumerable<Domain.Entities.Order>> GetOrdersAsync(string userId)
    {
        return await context.Orders
            .Include(o => o.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.Date)
            .ToListAsync();
    }

    public async Task<Domain.Entities.Order?> GetByIdAsync(Guid id)
    {
        return await context.Orders
            .Include(o => o.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task AddAsync(Domain.Entities.Order order)
    {
        await context.Orders.AddAsync(order);
    }

    public async Task UpdateAsync(Domain.Entities.Order order)
    {
        context.Orders.Update(order);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
