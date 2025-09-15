using Microsoft.EntityFrameworkCore;
using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;
using TheHireFactory.ECommerce.Infrastructure;
using TheHireFactory.ECommerce.Infrastructure.Data;
using TheHireFactory.ECommerce.Api.Mappings;
using TheHireFactory.ECommerce.Api.Middlewares;
using TheHireFactory.ECommerce.Api.Dtos;

var builder = WebApplication.CreateBuilder(args);

// --- Serilog ---
builder.Host.UseSerilog((ctx, lc) => lc
    .Enrich.WithEnvironmentName()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

// --- DB Connection ---
var cs = Environment.GetEnvironmentVariable("DB_CONNECTION")
         ?? builder.Configuration.GetConnectionString("ECommerce");
builder.Services.AddInfrastructure(cs);

// --- MVC + Controllers ---
builder.Services.AddControllers();

// --- Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Exception Handler + ProblemDetails ---
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// --- AutoMapper + Validators ---
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ProductCreateDto>();

// --- Logging ---
builder.Services.AddHttpLogging(_ => { /* varsayılan */ });

var app = builder.Build();

// --- Middleware pipeline ---
app.UseSerilogRequestLogging();
app.UseHttpLogging();

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok", time = DateTimeOffset.UtcNow }));

// --- DB migrate + seed (retry’li) ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
    const int maxRetries = 10;
    var delay = TimeSpan.FromSeconds(3);

    for (var attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            await db.Database.MigrateAsync();
            await SeedData.EnsureSeedAsync(db);
            app.Logger.LogInformation("Database migration applied (attempt {Attempt})", attempt);
            break;
        }
        catch (Exception ex) when (attempt < maxRetries)
        {
            app.Logger.LogWarning(ex, "DB not ready yet, retrying in {Delay}s... (attempt {Attempt}/{Max})",
                delay.TotalSeconds, attempt, maxRetries);
            await Task.Delay(delay);
        }
    }
}

app.Run();