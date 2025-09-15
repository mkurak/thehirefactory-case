using Microsoft.EntityFrameworkCore;
using TheHireFactory.ECommerce.Domain.Entities;
using TheHireFactory.ECommerce.Infrastructure.Repositories;

namespace TheHireFactory.ECommerce.Tests.Unit;

public class ProductRepositoryTests
{
    [Fact]
    public async Task AddAsync_Should_Persist_Product()
    {
        await using var db = TestDb.NewContext();
        var repo = new ProductRepository(db);

        var cat = new Category { Name = "Elektronik" };
        db.Categories.Add(cat);
        await db.SaveChangesAsync();

        var p = new Product { Name = "Laptop", Price = 19999.90m, Stock = 10, CategoryId = cat.Id };
        var created = await repo.AddAsync(p);

        created.Id.Should().BeGreaterThan(0);
        (await db.Products.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task ListWithCategoryAsync_Should_Include_Category()
    {
        await using var db = TestDb.NewContext();
        var repo = new ProductRepository(db);

        var cat = new Category { Name = "Aksesuar" };
        db.Categories.Add(cat);
        db.Products.Add(new Product { Name = "Mouse", Price = 499.90m, Stock = 50, CategoryId = cat.Id });
        await db.SaveChangesAsync();

        var list = await repo.ListWithCategoryAsync();

        list.Should().HaveCount(1);
        list[0].Category.Should().NotBeNull();
        list[0].Category!.Name.Should().Be("Aksesuar");
    }
}