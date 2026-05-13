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
            new() { 
                Id = Guid.Parse("b1111111-1111-1111-1111-111111111111"), 
                Name = "Big Mac Meal", 
                Description = "The iconic Big Mac Meal - a double layer of sear-sizzled 100% pure beef mingled with special sauce.",
                Price = 399.00m, 
                ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=500&q=80",
                BusinessId = "mcdonalds-01" 
            },
            new() { 
                Id = Guid.Parse("b2222222-2222-2222-2222-222222222222"), 
                Name = "Whopper Combo", 
                Description = "Our signature Whopper sandwich with a large side of French fries and a drink.",
                Price = 420.00m, 
                ImageUrl = "https://images.unsplash.com/photo-1571091718767-18b5b1457add?w=500&q=80",
                BusinessId = "burgerking-01" 
            },
            new() { 
                Id = Guid.Parse("b3333333-3333-3333-3333-333333333333"), 
                Name = "Chicken Sandwich", 
                Description = "Crispy, juicy chicken breast on a toasted brioche bun with pickles and mayo.",
                Price = 245.00m, 
                ImageUrl = "https://images.unsplash.com/photo-1606755962773-d324e0a13086?w=500&q=80",
                BusinessId = "popeyes-01" 
            }
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

            string category = bizName switch {
                "SHOco." => "Bakery",
                "Svoyi Bakery" => "Bakery",
                "Galician Bakery" => "Bakery",
                "Prostir Coffee & Pastry" => "Bakery",
                "Silpo Rynok" => "Groceries",
                "ATB Market" => "Groceries",
                "Rukavychka" => "Groceries",
                "Baczewski Restaurant" => "Meals",
                "Urban Food Lviv" => "Meals",
                "Grand Cafe Leopolis" => "Desserts",
                "Lviv Cakes" => "Desserts",
                "Meat Market №1" => "Meat & Fish",
                "Fish Hub" => "Meat & Fish",
                "Fresh Veggie Lviv" => "Vegetables",
                "Eco Lavka" => "Vegetables",
                _ => "Groceries"
            };

            string[] productImages = category switch {
                "Bakery" => new[] { "https://images.unsplash.com/photo-1550617931-e17a7b70dce2?w=500&q=80", "https://images.unsplash.com/photo-1608198093002-ad4e005484ec?w=500&q=80" },
                "Groceries" => new[] { "https://images.unsplash.com/photo-1583258292688-d0213dc5a3a8?w=500&q=80", "https://images.unsplash.com/photo-1578916171728-46686eac8d58?w=500&q=80" },
                "Meals" => new[] { "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=500&q=80", "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=500&q=80" },
                "Desserts" => new[] { "https://images.unsplash.com/photo-1563729784474-d77dbb933a9e?w=500&q=80", "https://images.unsplash.com/photo-1587314168485-3236d6710814?w=500&q=80" },
                "Meat & Fish" => new[] { "https://images.unsplash.com/photo-1529692236671-f1f6cf9683ba?w=500&q=80", "https://images.unsplash.com/photo-1615141982883-c7ad0e69fd62?w=500&q=80" },
                "Vegetables" => new[] { "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=500&q=80", "https://images.unsplash.com/photo-1590779033100-9f60a05a01bc?w=500&q=80" },
                _ => new[] { "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=500&q=80", "https://images.unsplash.com/photo-1476224203421-9ac39bcb3327?w=500&q=80" }
            };

            for (int i = 1; i <= 2; i++)
            {
                products.Add(new Product
                {
                    Id = Guid.Parse($"602d2149-e773-f2a3-990b-47e{productIndex:D2}0000000"),
                    BusinessId = businessId,
                    Name = $"{bizName} Surprise Bag {i}",
                    Description = $"Свіжа пропозиція від {bizName}. Тільки сьогодні!",
                    ImageUrl = productImages[(i - 1) % productImages.Length],
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
