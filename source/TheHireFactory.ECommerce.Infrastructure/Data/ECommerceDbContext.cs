using Microsoft.EntityFrameworkCore;
using TheHireFactory.ECommerce.Domain.Entities;

namespace TheHireFactory.ECommerce.Infrastructure.Data;

public class ECommerceDbContext(DbContextOptions<ECommerceDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(b =>
        {
            b.ToTable("Categories");
            b.HasKey(e => e.Id);
            b.Property(e => e.Name).IsRequired().HasMaxLength(200);
            b.HasMany(e => e.Products).WithOne(x => x.Category!).HasForeignKey(x => x.CategoryId);
        });

        modelBuilder.Entity<Product>(b =>
        {
            b.ToTable("Products");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.Price).HasColumnType("decimal(18,2)");
        });
    }
}