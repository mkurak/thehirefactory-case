using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TheHireFactory.ECommerce.Api;
using TheHireFactory.ECommerce.Infrastructure.Data;
using TheHireFactory.ECommerce.Domain.Entities;
using Microsoft.Extensions.Configuration;
using TheHireFactory.ECommerce.Domain.Abstractions;
using TheHireFactory.ECommerce.Infrastructure.Repositories;

namespace TheHireFactory.ECommerce.Tests.Integration;

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>, IDisposable
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((ctx, cfg) =>
    {
        cfg.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["DisableSeed"] = "true"
        });
    });

        builder.ConfigureServices(services =>
        {
            services.AddScoped<IProductRepository, ProductRepository>();

            var dbOptDesc = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ECommerceDbContext>));
            if (dbOptDesc is not null) services.Remove(dbOptDesc);
            services.RemoveAll(typeof(ECommerceDbContext));

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<ECommerceDbContext>(opt => opt.UseSqlite(_connection));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
            db.Database.EnsureCreated();

            if (!db.Categories.Any())
            {
                db.Categories.Add(new Category { Id = 1, Name = "Default" });
                db.SaveChanges();
            }
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection?.Dispose();
            _connection = null;
        }
    }
}