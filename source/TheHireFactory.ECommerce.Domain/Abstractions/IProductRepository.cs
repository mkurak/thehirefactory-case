using TheHireFactory.ECommerce.Domain.Entities;

namespace TheHireFactory.ECommerce.Domain.Abstractions;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> ListWithCategoryAsync(CancellationToken ct = default);
    Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Product> AddAsync(Product entity, CancellationToken ct = default);
}