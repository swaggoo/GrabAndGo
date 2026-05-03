using GrabAndGo.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GrabAndGo.Catalog.Infrastructure.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Business> Businesses { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<SavedProduct> SavedProducts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SavedProduct>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.ProductId }).IsUnique();
            entity.HasOne(e => e.Product)
                  .WithMany()
                  .HasForeignKey(e => e.ProductId);
        });

        modelBuilder.Entity<Business>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BusinessId).IsRequired();
            entity.HasIndex(e => e.BusinessId).IsUnique();
            entity.OwnsOne(e => e.Address);
            entity.OwnsOne(e => e.Location);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Business)
                  .WithMany(b => b.Products)
                  .HasForeignKey(e => e.BusinessId)
                  .HasPrincipalKey(b => b.BusinessId);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }
}
