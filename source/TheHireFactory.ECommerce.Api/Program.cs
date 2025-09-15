using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using TheHireFactory.ECommerce.Infrastructure;
using Microsoft.OpenApi.Models;
using TheHireFactory.ECommerce.Infrastructure;
using Microsoft.Extensions.Logging;
using TheHireFactory.ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var cs = Environment.GetEnvironmentVariable("DB_CONNECTION") ?? builder.Configuration.GetConnectionString("ECommerce");

builder.Services.AddInfrastructure(cs);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ECommerce API", Version = "v1" });
});
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", time = DateTimeOffset.UtcNow }));

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
        await db.Database.MigrateAsync();
        app.Logger.LogInformation("Database migration applied.");

        await SeedData.EnsureSeedAsync(db);
        app.Logger.LogInformation("Database seeding completed.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Database migration and seeding failed.");
        throw;
    }
}

app.Run();
