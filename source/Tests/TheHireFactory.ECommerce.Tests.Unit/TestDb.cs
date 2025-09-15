using Microsoft.EntityFrameworkCore;
using TheHireFactory.ECommerce.Infrastructure.Data;

namespace TheHireFactory.ECommerce.Tests.Unit;

public static class TestDb
{
    public static ECommerceDbContext NewContext()
    {
        var options = new DbContextOptionsBuilder<ECommerceDbContext>()
            .UseInMemoryDatabase($"ecommerce-tests-{Guid.NewGuid()}")
            .EnableSensitiveDataLogging()
            .Options;

        return new ECommerceDbContext(options);
    }
}