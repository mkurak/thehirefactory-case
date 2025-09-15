using Microsoft.EntityFrameworkCore;
using TheHireFactory.ECommerce.Domain.Abstractions;
using TheHireFactory.ECommerce.Domain.Entities;
using TheHireFactory.ECommerce.Infrastructure.Data;

namespace TheHireFactory.ECommerce.Infrastructure.Repositories;

public class ProductRepository : RepositoryBase<Product>, IProductRepository
{
    public ProductRepository(ECommerceDbContext db) : base(db) { }

    public async Task<IReadOnlyList<Product>> ListWithCategoryAsync(CancellationToken ct = default)
        => await _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .ToListAsync(ct);

    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Product> AddAsync(Product entity, CancellationToken ct = default)
    {
        await _db.Products.AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);

        await _db.Entry(entity).Reference(p => p.Category).LoadAsync(ct);
        return entity;
    }
}