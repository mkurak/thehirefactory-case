using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TheHireFactory.ECommerce.Domain.Abstractions;
using TheHireFactory.ECommerce.Infrastructure.Data;
using TheHireFactory.ECommerce.Infrastructure.Repositories;

namespace TheHireFactory.ECommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string? connectionString)
    {
        services.AddDbContext<ECommerceDbContext>(opt => opt.UseSqlServer(connectionString ?? throw new InvalidOperationException("Connection string is not provided.")));

        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}