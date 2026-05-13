using GrabAndGo.Identity.API.Models;
using Microsoft.AspNetCore.Identity;

namespace GrabAndGo.Identity.API.Data;

public static class IdentityDataSeed
{
    public static async Task SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Seed Roles
        string[] roles = { "Business", "Customer" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Seed SHOco. Business Account
        var shocoId = "602d2149-e773-f2a3-990b-47b000000000";
        var shocoUser = await userManager.FindByIdAsync(shocoId);
        if (shocoUser == null)
        {
            shocoUser = new ApplicationUser
            {
                Id = shocoId,
                UserName = "shoco.bakery",
                Email = "shoco.lviv@gmail.com",
                EmailConfirmed = true,
                BusinessName = "SHOco.",
                BusinessDescription = "Сучасна кондитерська з відкритою кухнею. Ми печемо найкращі круасани, робимо легендарні макарони та готуємо сніданки весь день. Наша місія — дарувати емоції через смак.",
                BusinessCategory = "Bakery",
                BusinessPhone = "+380504307575",
                BusinessWebsite = "https://shoco.ua/",
                LogoUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRiQsv7tmwdnYtMA2duXmrHPglFBZmb79zMtg&s",
                CoverImageUrl = "https://tickikids.ams3.cdn.digitaloceanspaces.com/z1.cache/gallery/organizations/1579/image_5aa586241ed0d6.29769662.jpg",
                Street = "Uhorska St, 12",
                City = "Lviv",
                PostalCode = "79000",
                Country = "Ukraine",
                Latitude = 49.8120223,
                Longitude = 24.0403886,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(shocoUser, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(shocoUser, "Business");
            }
        }

        // Seed Mock Customer Account
        var mockUserId = "6611c298-f744-421e-88b5-99369ce67e52";
        var mockUser = await userManager.FindByIdAsync(mockUserId);
        if (mockUser == null)
        {
            mockUser = new ApplicationUser
            {
                Id = mockUserId,
                UserName = "johndoe",
                Email = "john.doe@example.com",
                EmailConfirmed = true,
                FirstName = "John",
                LastName = "Doe",
                AvatarUrl = "https://images.unsplash.com/photo-1599566150163-29194dcaad36?w=256&h=256&fit=crop&q=80",
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(mockUser, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(mockUser, "Customer");
            }
        }

        // Seed Another Mock Customer Account
        var mockUserId2 = "7722d3a9-0855-532f-99c6-0047a0000001";
        var mockUser2 = await userManager.FindByIdAsync(mockUserId2);
        if (mockUser2 == null)
        {
            mockUser2 = new ApplicationUser
            {
                Id = mockUserId2,
                UserName = "janedoe",
                Email = "jane.doe@example.com",
                EmailConfirmed = true,
                FirstName = "Jane",
                LastName = "Doe",
                AvatarUrl = "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=256&h=256&fit=crop&q=80",
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(mockUser2, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(mockUser2, "Customer");
            }
        }
    }
}
