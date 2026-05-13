using GrabAndGo.Order.Domain.Entities;
using GrabAndGo.Order.Domain.Enums;
using GrabAndGo.Order.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GrabAndGo.Order.Infrastructure.Data;

public static class OrderContextSeed
{
    public static void SeedData(OrderDbContext context)
    {
        // Clear existing data to ensure updates (prices, new orders) are applied
        context.Orders.RemoveRange(context.Orders);
        context.Products.RemoveRange(context.Products);
        context.SaveChanges();

        context.Products.AddRange(GetPreconfiguredProducts());
        context.SaveChanges();

        var products = context.Products.ToList();

        context.Orders.AddRange(GetPreconfiguredOrders(products));
        
        const string mockUserId = "6611c298-f744-421e-88b5-99369ce67e52";
        context.Orders.AddRange(GetOrdersForMockUser(mockUserId, products));
        
        context.Orders.AddRange(GetShocoSpecificOrders(products));
        
        context.SaveChanges();
    }

    private static IEnumerable<Domain.Entities.Order> GetShocoSpecificOrders(List<Product> products)
    {
        var shocoId = "602d2149-e773-f2a3-990b-47b000000000";
        var shocoProducts = products.Where(p => p.BusinessId == shocoId).ToList();
        
        if (!shocoProducts.Any()) return Enumerable.Empty<Domain.Entities.Order>();

        var random = new Random();
        var customerIds = new[] 
        { 
            "6611c298-f744-421e-88b5-99369ce67e52", // John Doe
            "7722d3a9-0855-532f-99c6-0047a0000001", // Jane Doe
            "8833e4b0-1966-643g-00d7-1158b1111112"  // Mock User 2 (External)
        };

        var orders = new List<Domain.Entities.Order>();

        // Completed orders
        for (int i = 0; i < 5; i++)
        {
            var product = shocoProducts[random.Next(shocoProducts.Count)];
            orders.Add(new Domain.Entities.Order
            {
                Id = Guid.NewGuid(),
                OrderNum = $"#S{random.Next(1000, 9999)}",
                UserId = customerIds[random.Next(customerIds.Length)],
                BusinessId = shocoId,
                ProductId = product.Id,
                TotalAmount = product.Price,
                Status = OrderStatus.COMPLETED,
                Date = DateTime.UtcNow.AddDays(-random.Next(1, 10)).AddHours(-random.Next(1, 24))
            });
        }

        // Active orders
        orders.Add(new Domain.Entities.Order
        {
            Id = Guid.NewGuid(),
            OrderNum = $"#S{random.Next(1000, 9999)}",
            UserId = customerIds[0],
            BusinessId = shocoId,
            ProductId = shocoProducts[0].Id,
            TotalAmount = shocoProducts[0].Price,
            Status = OrderStatus.READY_FOR_PICKUP,
            Date = DateTime.UtcNow.AddHours(-2)
        });

        orders.Add(new Domain.Entities.Order
        {
            Id = Guid.NewGuid(),
            OrderNum = $"#S{random.Next(1000, 9999)}",
            UserId = customerIds[1],
            BusinessId = shocoId,
            ProductId = shocoProducts[1 % shocoProducts.Count].Id,
            TotalAmount = shocoProducts[1 % shocoProducts.Count].Price,
            Status = OrderStatus.ACCEPTED,
            Date = DateTime.UtcNow.AddMinutes(-45)
        });

        orders.Add(new Domain.Entities.Order
        {
            Id = Guid.NewGuid(),
            OrderNum = $"#S{random.Next(1000, 9999)}",
            UserId = customerIds[2],
            BusinessId = shocoId,
            ProductId = shocoProducts[0].Id,
            TotalAmount = shocoProducts[0].Price,
            Status = OrderStatus.NEW,
            Date = DateTime.UtcNow.AddMinutes(-10)
        });

        return orders;
    }

    private static IEnumerable<Domain.Entities.Order> GetOrdersForMockUser(string userId, List<Product> products)
    {
        var random = new Random();
        var shocoProducts = products.Where(p => p.BusinessId == "602d2149-e773-f2a3-990b-47b000000000").ToList();
        
        var orders = new List<Domain.Entities.Order>
        {
            new() 
            { 
                Id = Guid.NewGuid(), 
                OrderNum = $"#{random.Next(1000, 9999)}", 
                UserId = userId, 
                BusinessId = products[0].BusinessId,
                ProductId = products[0].Id, 
                TotalAmount = products[0].Price, 
                Status = OrderStatus.COMPLETED, 
                Date = DateTime.UtcNow.AddDays(-5) 
            },
            new() 
            { 
                Id = Guid.NewGuid(), 
                OrderNum = $"#{random.Next(1000, 9999)}", 
                UserId = userId, 
                BusinessId = products[1].BusinessId,
                ProductId = products[1].Id, 
                TotalAmount = products[1].Price, 
                Status = OrderStatus.COMPLETED, 
                Date = DateTime.UtcNow.AddDays(-3) 
            }
        };

        // Add orders from Shoco Bakery if products exist
        if (shocoProducts.Any())
        {
            orders.Add(new Domain.Entities.Order 
            { 
                Id = Guid.NewGuid(), 
                OrderNum = $"#{random.Next(1000, 9999)}", 
                UserId = userId, 
                BusinessId = shocoProducts[0].BusinessId,
                ProductId = shocoProducts[0].Id, 
                TotalAmount = shocoProducts[0].Price, 
                Status = OrderStatus.READY_FOR_PICKUP, 
                Date = DateTime.UtcNow.AddHours(-1) 
            });

            if (shocoProducts.Count > 1)
            {
                orders.Add(new Domain.Entities.Order 
                { 
                    Id = Guid.NewGuid(), 
                    OrderNum = $"#{random.Next(1000, 9999)}", 
                    UserId = userId, 
                    BusinessId = shocoProducts[1].BusinessId,
                    ProductId = shocoProducts[1].Id, 
                    TotalAmount = shocoProducts[1].Price, 
                    Status = OrderStatus.COMPLETED, 
                    Date = DateTime.UtcNow.AddDays(-2) 
                });
            }
        }

        return orders;
    }

    private static IEnumerable<Product> GetPreconfiguredProducts()
    {
        var products = new List<Product>
        {
            new() { Id = Guid.Parse("b1111111-1111-1111-1111-111111111111"), Name = "Big Mac Meal", Price = 399.00m, BusinessId = "mcdonalds-01" },
            new() { Id = Guid.Parse("b2222222-2222-2222-2222-222222222222"), Name = "Whopper Combo", Price = 420.00m, BusinessId = "burgerking-01" },
            new() { Id = Guid.Parse("b3333333-3333-3333-3333-333333333333"), Name = "Chicken Sandwich", Price = 245.00m, BusinessId = "popeyes-01" }
        };

        var lvivBusinesses = new List<string>
        {
            "SHOco.", "Silpo Rynok", "Baczewski Restaurant", "Svoyi Bakery", "ATB Market",
            "Meat Market №1", "Fresh Veggie Lviv", "Grand Cafe Leopolis", "Rukavychka", "Lviv Cakes",
            "Eco Lavka", "Fish Hub", "Galician Bakery", "Urban Food Lviv", "Prostir Coffee & Pastry"
        };

        int productIndex = 0;
        for (int bizIndex = 0; bizIndex < lvivBusinesses.Count; bizIndex++)
        {
            var bizName = lvivBusinesses[bizIndex];
            var businessId = $"602d2149-e773-f2a3-990b-47b{bizIndex:D2}0000000";

            for (int i = 1; i <= 2; i++)
            {
                products.Add(new Product
                {
                    Id = Guid.Parse($"602d2149-e773-f2a3-990b-47e{productIndex:D2}0000000"),
                    BusinessId = businessId,
                    Name = $"{bizName} Surprise Bag {i}",
                    Price = 199.00M + (productIndex * 20)
                });
                productIndex++;
            }
        }

        return products;
    }

    private static IEnumerable<Domain.Entities.Order> GetPreconfiguredOrders(List<Product> products)
    {
        var random = new Random();
        return new List<Domain.Entities.Order>
        {
            new() 
            { 
                Id = Guid.NewGuid(), 
                OrderNum = "#1110", 
                UserId = "user-01", 
                BusinessId = products[0].BusinessId,
                ProductId = products[0].Id, 
                TotalAmount = products[0].Price, 
                Status = OrderStatus.COMPLETED, 
                Date = DateTime.UtcNow.AddDays(-2) 
            },
            new() 
            { 
                Id = Guid.NewGuid(), 
                OrderNum = $"#{random.Next(1000, 9999)}", 
                UserId = "user-01", 
                BusinessId = products[1].BusinessId,
                ProductId = products[1].Id, 
                TotalAmount = products[1].Price, 
                Status = OrderStatus.READY_FOR_PICKUP, 
                Date = DateTime.UtcNow.AddHours(-1) 
            },
            new() 
            { 
                Id = Guid.NewGuid(), 
                OrderNum = $"#{random.Next(1000, 9999)}", 
                UserId = "user-01", 
                BusinessId = products[2].BusinessId,
                ProductId = products[2].Id, 
                TotalAmount = products[2].Price, 
                Status = OrderStatus.NEW, 
                Date = DateTime.UtcNow 
            }
        };
    }
}
