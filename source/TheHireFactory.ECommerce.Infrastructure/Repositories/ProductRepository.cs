using Microsoft.EntityFrameworkCore;
using TheHireFactory.ECommerce.Domain.Abstractions;
using TheHireFactory.ECommerce.Domain.Entities;
using TheHireFactory.ECommerce.Infrastructure.Data;

namespace TheHireFactory.ECommerce.Infrastructure.Repositories;

public class ProductRepository(ECommerceDbContext db) : RepositoryBase<Product>(db), IProductRepository
{
    public async Task<IReadOnlyList<Product>> ListWithCategoryAsync()
        => await _db.Products.AsNoTracking().Include(p => p.Category).ToListAsync();
}