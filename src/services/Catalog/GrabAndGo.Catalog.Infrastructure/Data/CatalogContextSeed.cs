using GrabAndGo.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GrabAndGo.Catalog.Infrastructure.Data;

public class CatalogContextSeed
{
    private class BusinessSeedInfo
    {
        public Business Business { get; set; } = default!;
        public string CategoryName { get; set; } = default!;
    }

    public static void SeedData(CatalogDbContext context)
    {
        context.Products.RemoveRange(context.Products);
        context.Businesses.RemoveRange(context.Businesses);
        context.Categories.RemoveRange(context.Categories);
        context.SaveChanges();

        var categories = GetPreconfiguredCategories().ToList();
        context.Categories.AddRange(categories);
       
        var businessInfos = GetPreconfiguredBusinessInfos().ToList();
        context.Businesses.AddRange(businessInfos.Select(x => x.Business));
       
        context.SaveChanges();

        var products = GetPreconfiguredProducts(businessInfos, categories).ToList();
        context.Products.AddRange(products);
       
        context.SaveChanges();
    }

    private static IEnumerable<Category> GetPreconfiguredCategories()
    {
        return new List<Category>
        {
            new() { Id = Guid.Parse("602d2149-e773-f2a3-990b-47a100000000"), Name = "Bakery" },
            new() { Id = Guid.Parse("602d2149-e773-f2a3-990b-47a200000000"), Name = "Groceries" },
            new() { Id = Guid.Parse("602d2149-e773-f2a3-990b-47a300000000"), Name = "Meals" },
            new() { Id = Guid.Parse("602d2149-e773-f2a3-990b-47a400000000"), Name = "Vegetables" },
            new() { Id = Guid.Parse("602d2149-e773-f2a3-990b-47a500000000"), Name = "Meat & Fish" },
            new() { Id = Guid.Parse("602d2149-e773-f2a3-990b-47a600000000"), Name = "Desserts" }
        };
    }

    private static IEnumerable<BusinessSeedInfo> GetPreconfiguredBusinessInfos()
    {
        var lvivBusinesses = new List<(string Name, string Cat, string Street, string Lat, string Long)>
        {
            ("Lvivska Maisternia Shokoladu", "Desserts", "Serbska St, 3", "49.8413", "24.0322"),
            ("Silpo Rynok", "Groceries", "Ploshcha Rynok, 1", "49.8417", "24.0312"),
            ("Baczewski Restaurant", "Meals", "Shevska St, 8", "49.8423", "24.0305"),
            ("Svoyi Bakery", "Bakery", "Staroievreiska St, 24", "49.8408", "24.0315"),
            ("ATB Market", "Groceries", "Horodotska St, 16", "49.8425", "24.0210"),
            ("Meat Market №1", "Meat & Fish", "Virmenska St, 5", "49.8432", "24.0318"),
            ("Fresh Veggie Lviv", "Vegetables", "Stryiska St, 45", "49.8175", "24.0234"),
            ("Grand Cafe Leopolis", "Desserts", "Rynok Square, 10", "49.8415", "24.0326"),
            ("Rukavychka", "Groceries", "Shevchenka St, 20", "49.8450", "24.0150"),
            ("Lviv Cakes", "Desserts", "Kulisha St, 12", "49.8465", "24.0255"),
            ("Eco Lavka", "Vegetables", "Chornovola Ave, 67", "49.8580", "24.0200"),
            ("Fish Hub", "Meat & Fish", "Saharova St, 42", "49.8280", "24.0120"),
            ("Galician Bakery", "Bakery", "Ivana Franka St, 15", "49.8360", "24.0340"),
            ("Urban Food Lviv", "Meals", "Krakivska St, 9", "49.8435", "24.0308"),
            ("Prostir Coffee & Pastry", "Bakery", "Doroshenka St, 22", "49.8402", "24.0275")
        };

        return lvivBusinesses.Select((b, index) => {
            string coverUrl = b.Cat switch {
                "Bakery" => "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=800&q=80",
                "Groceries" => "https://images.unsplash.com/photo-1542838132-92c53300491e?w=800&q=80",
                "Meals" => "https://images.unsplash.com/photo-1514933651103-005eec06c04b?w=800&q=80",
                "Desserts" => "https://images.unsplash.com/photo-1488477181946-6428a0291777?w=800&q=80",
                "Meat & Fish" => "https://images.unsplash.com/photo-1603048297172-c92544798d5e?w=800&q=80",
                "Vegetables" => "https://images.unsplash.com/photo-1566385101042-1a0aa0c1268c?w=800&q=80",
                _ => "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=800&q=80"
            };

            string logoUrl = b.Cat switch {
                "Bakery" => "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=256&h=256&fit=crop&q=80",
                "Groceries" => "https://images.unsplash.com/photo-1578916171728-46686eac8d58?w=256&h=256&fit=crop&q=80",
                "Meals" => "https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=256&h=256&fit=crop&q=80",
                "Desserts" => "https://images.unsplash.com/photo-1563729784474-d77dbb933a9e?w=256&h=256&fit=crop&q=80",
                "Meat & Fish" => "https://images.unsplash.com/photo-1607623814075-e51df1bdc82f?w=256&h=256&fit=crop&q=80",
                "Vegetables" => "https://images.unsplash.com/photo-1597362925123-77861d3fbac7?w=256&h=256&fit=crop&q=80",
                _ => "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=256&h=256&fit=crop&q=80"
            };

            var businessGuid = Guid.Parse($"602d2149-e773-f2a3-990b-47b{index:D2}0000000");

            return new BusinessSeedInfo
            {
                CategoryName = b.Cat,
                Business = new Business
                {
                    Id = businessGuid,
                    BusinessId = businessGuid.ToString(),
                    Name = b.Name,
                    Description = $"Найкращий вибір у категорії {b.Cat} у Львові",
                    LogoUrl = logoUrl,
                    CoverImageUrl = coverUrl,
                    Address = new BusinessAddress { Street = b.Street, City = "Lviv", PostalCode = "79000", Country = "Ukraine" },
                    Location = new BusinessLocation { Latitude = double.Parse(b.Lat), Longitude = double.Parse(b.Long) },
                    IsActive = true
                }
            };
        });
    }

    private static IEnumerable<Product> GetPreconfiguredProducts(IEnumerable<BusinessSeedInfo> businessInfos, List<Category> categories)
    {
        var products = new List<Product>();
        int productIndex = 0;

        foreach (var info in businessInfos)
        {
            var biz = info.Business;
            var category = categories.FirstOrDefault(c => c.Name == info.CategoryName) ?? categories.First();

            string[] productImages = info.CategoryName switch {
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
                    Id = Guid.Parse($"602d2149-e773-f2a3-990b-47c{productIndex:D2}0000000"),
                    BusinessId = biz.BusinessId,
                    CategoryId = category.Id.ToString(),
                    Name = $"{biz.Name} Surprise Bag {i}",
                    Description = $"Свіжа пропозиція від {biz.Name}. Тільки сьогодні!",
                    ImageUrl = productImages[i - 1],
                    Price = 50.00M + (productIndex * 5),
                    OriginalPrice = 150.00M + (productIndex * 5),
                    Quantity = 2 + i,
                    PickupStart = DateTime.UtcNow.AddHours(1),
                    PickupEnd = DateTime.UtcNow.AddHours(4),
                    IsActive = true,
                    Rating = GenerateRandomRating()
                });
                productIndex++;
            }
        }

        return products;
    }

    private static Rating GenerateRandomRating()
    {
        var random = new Random();
        float overall = (float)(random.NextDouble() * 3.5 + 1.5); // 1.5 to 5.0
        return new Rating
        {
            OverallRating = (float)Math.Round(overall, 1),
            TotalRatings = random.Next(1, 100),
            CollectionRating = (float)Math.Round(random.NextDouble() * 3.5 + 1.5, 1),
            QualityRating = (float)Math.Round(random.NextDouble() * 3.5 + 1.5, 1),
            VarietyRating = (float)Math.Round(random.NextDouble() * 3.5 + 1.5, 1),
            QuantityRating = (float)Math.Round(random.NextDouble() * 3.5 + 1.5, 1)
        };
    }
}