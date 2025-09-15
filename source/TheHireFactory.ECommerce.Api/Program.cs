using Microsoft.OpenApi.Models;
using TheHireFactory.ECommerce.Infrastructure;
using TheHireFactory.ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.HttpLogging;
using FluentValidation;
using FluentValidation.AspNetCore;
using AutoMapper;
using TheHireFactory.ECommerce.Api.Mappings;
using TheHireFactory.ECommerce.Api.Dtos;

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
builder.Services.AddHttpLogging(o => { o.LoggingFields = HttpLoggingFields.All; });
builder.Services.AddExceptionHandler<TheHireFactory.ECommerce.Api.Middlewares.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddFluentValidationAutoValidation(); // otomatik model validation
builder.Services.AddValidatorsFromAssemblyContaining<ProductCreateDto>(); // assembly scan

builder.Host.UseSerilog((ctx, lc) => lc
    .Enrich.WithEnvironmentName()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.UseExceptionHandler();

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
