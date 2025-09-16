using Microsoft.EntityFrameworkCore;
using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;
using TheHireFactory.ECommerce.Infrastructure;
using TheHireFactory.ECommerce.Infrastructure.Data;
using TheHireFactory.ECommerce.Api.Mappings;
using TheHireFactory.ECommerce.Api.Middlewares;
using TheHireFactory.ECommerce.Api.Dtos;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Filters;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using TheHireFactory.ECommerce.Api.Swagger;

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

// ⚠️ Test ortamında AddInfrastructure çağrılmasın; TestWebApplicationFactory SQLite'ı kendisi ekleyecek
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddInfrastructure(cs);
}

// --- MVC + Controllers ---
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// --- Swagger ---
builder.Services.AddFluentValidationRulesToSwagger();
builder.Services.AddSwaggerExamplesFromAssemblyOf<ProductCreateExample>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ECommerce API",
        Version = "v1",
        Description = "Ürün / kategori yönetimi için minimal e-ticaret API’si",
        Contact = new OpenApiContact { Name = "THF Team" }
    });

    var xml = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xml);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

    c.ExampleFilters();

    c.OperationFilter<DefaultResponsesOperationFilter>();

    c.CustomSchemaIds(t => t.FullName!.Replace("+", "."));
});

// --- Exception Handler + ProblemDetails ---
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// --- AutoMapper + Validators ---
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ProductCreateDto>();

// --- Logging ---
builder.Services.AddHttpLogging(_ => { /* defaults */ });

var app = builder.Build();

// --- Middleware pipeline ---
app.UseSerilogRequestLogging();
app.UseHttpLogging();

if (app.Environment.IsEnvironment("Testing"))
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok", time = DateTimeOffset.UtcNow }));

// --- DB migrate + seed (retry’li) ---
// Test ortamında migrate/seed çalışmasın; ayrıca appsettings ile DisableSeed=true ise de atla
var disableSeed = builder.Configuration.GetValue<bool>("DisableSeed", false);
if (!builder.Environment.IsEnvironment("Testing") && !disableSeed)
{
    using var scope = app.Services.CreateScope();
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