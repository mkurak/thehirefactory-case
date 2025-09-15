using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TheHireFactory.ECommerce.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ECommerceDbContext>
{
    public ECommerceDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ECommerceDbContext>();
        var cs = Environment.GetEnvironmentVariable("DB_CONNECTION") ?? "Server=sqlserver;Database=TheHireFactoryECommerceCaseDb;User Id=sa;Password=user123TEST.;TrustServerCertificate=True;";

        optionsBuilder.UseSqlServer(cs);

        return new ECommerceDbContext(optionsBuilder.Options);
    }
}