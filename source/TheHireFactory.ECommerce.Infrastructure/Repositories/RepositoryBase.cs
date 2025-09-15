using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TheHireFactory.ECommerce.Domain.Abstractions;
using TheHireFactory.ECommerce.Infrastructure.Data;

namespace TheHireFactory.ECommerce.Infrastructure.Repositories;

public class RepositoryBase<T>(ECommerceDbContext db) : IRepository<T> where T : class
{
    protected readonly ECommerceDbContext _db = db;

    public async Task<T?> GetByIdAsync(int id) => await _db.Set<T>().FindAsync(id);

    public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null)
        => predicate is null
            ? await _db.Set<T>().AsNoTracking().ToListAsync()
            : await _db.Set<T>().AsNoTracking().Where(predicate).ToListAsync();

    public async Task<T> AddAsync(T entity)
    {
        _db.Set<T>().Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _db.Set<T>().Update(entity);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _db.Set<T>().Remove(entity);
        await _db.SaveChangesAsync();
    }
}