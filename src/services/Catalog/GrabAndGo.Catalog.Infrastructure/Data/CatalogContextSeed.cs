using GrabAndGo.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GrabAndGo.Catalog.Infrastructure.Data;

public class CatalogContextSeed
{
    public static void SeedData(CatalogDbContext context)
    {
        // Clear existing data to ensure fresh mock data with categories
        context.Products.RemoveRange(context.Products);
        context.Businesses.RemoveRange(context.Businesses);
        context.Categories.RemoveRange(context.Categories);
        context.SaveChanges();

        var categories = GetPreconfiguredCategories();
        context.Categories.AddRange(categories);
        
        var businesses = GetPreconfiguredBusinesses();
        context.Businesses.AddRange(businesses);
        
        context.SaveChanges();

        var products = GetPreconfiguredProducts();
        context.Products.AddRange(products);
        
        context.SaveChanges();
    }

    private static IEnumerable<Category> GetPreconfiguredCategories()
    {
        return new List<Category>()
        {
            new Category() { Id = Guid.Parse("602d2149-e773-f2a3-990b-47a100000000"), Name = "Bakery" },
            new Category() { Id = Guid.Parse("602d2149-e773-f2a3-990b-47a200000000"), Name = "Groceries" },
            new Category() { Id = Guid.Parse("602d2149-e773-f2a3-990b-47a300000000"), Name = "Meals" },
            new Category() { Id = Guid.Parse("602d2149-e773-f2a3-990b-47a400000000"), Name = "Vegetables" }
        };
    }

    private static IEnumerable<Business> GetPreconfiguredBusinesses()
    {
        return new List<Business>()
        {
            new Business() 
            { 
                Id = Guid.Parse("602d2149-e773-f2a3-990b-47f000000000"), 
                BusinessId = "602d2149e773f2a3990b47e0",
                Name = "Green Grocery", 
                Description = "Fresh vegetables and fruits",
                Category = "Groceries",
                Address = new BusinessAddress { 
                    Street = "123 Green St", 
                    City = "Eco City", 
                    PostalCode = "10001", 
                    Country = "USA" 
                },
                Location = new BusinessLocation { 
                    Latitude = 40.7128, 
                    Longitude = -74.0060 
                },
                IsActive = true
            },
            new Business() 
            { 
                Id = Guid.Parse("602d2149-e773-f2a3-990b-47f100000000"), 
                BusinessId = "602d2149e773f2a3990b47e1",
                Name = "Bakery Bliss", 
                Description = "Delicious pastries and bread",
                Category = "Bakery",
                Address = new BusinessAddress { 
                    Street = "456 Baker Ave", 
                    City = "Sweet Town", 
                    PostalCode = "10002", 
                    Country = "USA" 
                },
                Location = new BusinessLocation { 
                    Latitude = 40.7306, 
                    Longitude = -73.9352 
                },
                IsActive = true
            }
        };
    }

    private static IEnumerable<Product> GetPreconfiguredProducts()
    {
        return new List<Product>()
        {
            new Product()
            {
                Id = Guid.Parse("602d2149-e773-f2a3-990b-47f500000000"),
                BusinessId = "602d2149e773f2a3990b47e0",
                CategoryId = Guid.Parse("602d2149-e773-f2a3-990b-47a400000000").ToString(), // Vegetables
                Name = "Vegetable Surprise Box",
                Description = "A mix of seasonal organic vegetables.",
                Price = 5.00M,
                OriginalPrice = 15.00M,
                Quantity = 5,
                PickupStart = DateTime.UtcNow.AddHours(2),
                PickupEnd = DateTime.UtcNow.AddHours(4),
                IsActive = true
            },
            new Product()
            {
                Id = Guid.Parse("602d2149-e773-f2a3-990b-47f600000000"),
                BusinessId = "602d2149e773f2a3990b47e1",
                CategoryId = Guid.Parse("602d2149-e773-f2a3-990b-47a100000000").ToString(), // Bakery
                Name = "Small Bakery Box",
                Description = "Assorted croissants and muffins.",
                Price = 4.00M,
                OriginalPrice = 12.00M,
                Quantity = 3,
                PickupStart = DateTime.UtcNow.AddHours(1),
                PickupEnd = DateTime.UtcNow.AddHours(3),
                IsActive = true
            },
            new Product()
            {
                Id = Guid.Parse("602d2149-e773-f2a3-990b-47f700000000"),
                BusinessId = "602d2149e773f2a3990b47e1",
                CategoryId = Guid.Parse("602d2149-e773-f2a3-990b-47a100000000").ToString(), // Bakery
                Name = "Large Bakery Box",
                Description = "A large variety of daily fresh bread and pastries.",
                Price = 8.00M,
                OriginalPrice = 25.00M,
                Quantity = 2,
                PickupStart = DateTime.UtcNow.AddHours(5),
                PickupEnd = DateTime.UtcNow.AddHours(7),
                IsActive = true
            }
        };
    }
}
