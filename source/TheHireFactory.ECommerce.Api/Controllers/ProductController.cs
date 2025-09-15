using Microsoft.AspNetCore.Mvc;
using TheHireFactory.ECommerce.Domain.Abstractions;
using TheHireFactory.ECommerce.Domain.Entities;

namespace TheHireFactory.ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductRepository products) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await products.ListWithCategoryAsync());

    [HttpPost]
    public async Task<IActionResult> Post(Product p)
    {
        var created = await products.AddAsync(p);

        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }
}