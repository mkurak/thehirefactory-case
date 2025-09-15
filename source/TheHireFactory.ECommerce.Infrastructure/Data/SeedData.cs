using TheHireFactory.ECommerce.Domain.Entities;

namespace TheHireFactory.ECommerce.Infrastructure.Data;

public static class SeedData
{
    public static async Task EnsureSeedAsync(ECommerceDbContext db)
    {
        if (!db.Categories.Any())
        {
            var cat = new Category { Name = "Yazılımcılara Özel" };
            db.Categories.Add(cat);
            db.Products.AddRange(
                new Product { Name = "Monster Abra A5 V16.5.1", Price = 34999, Category = cat, Stock = 10 },
                new Product { Name = "Asus TUF Gaming F15", Price = 24999, Category = cat, Stock = 15 },
                new Product { Name = "MacBook Pro 16", Price = 59999, Category = cat, Stock = 5 }
            );

            await db.SaveChangesAsync();
        }
    }
}