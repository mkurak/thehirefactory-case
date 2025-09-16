using Swashbuckle.AspNetCore.Filters;
using TheHireFactory.ECommerce.Api.Contracts;

namespace TheHireFactory.ECommerce.Api.Swagger;

public sealed class ProductCreateExample : IExamplesProvider<ProductCreateDto>
{
    public ProductCreateDto GetExamples() => new("Laptop", 19999.90m, 25, 1);
}