using TheHireFactory.ECommerce.Domain.Entities;

namespace TheHireFactory.ECommerce.Domain.Abstractions;

public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> ListWithCategoryAsync();
}