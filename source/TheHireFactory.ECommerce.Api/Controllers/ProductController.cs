using Microsoft.AspNetCore.Mvc;
using TheHireFactory.ECommerce.Api.Contracts;
using TheHireFactory.ECommerce.Domain.Abstractions;
using TheHireFactory.ECommerce.Domain.Entities;

namespace TheHireFactory.ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductRepository products) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var list = await products.ListWithCategoryAsync();

        var result = list.Select(p => new ProductReadDto(
            p.Id, p.Name, p.Price, p.Stock, p.CategoryId, p.Category!.Name
        ));

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var all = await products.ListWithCategoryAsync();
        var p = all.FirstOrDefault(x => x.Id == id);
        if (p is null)
        {
            return Problem(
                title: "Kayıt bulunamadı",
                detail: $"Product #{id} mevcut değil.",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        var dto = new ProductReadDto(
            p.Id, p.Name, p.Price, p.Stock, p.CategoryId, p.Category!.Name
        );

        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ProductCreateDto dto, CancellationToken ct)
    {
        var entity = new Product
        {
            Name = dto.Name.Trim(),
            Price = dto.Price,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId
        };

        var created = await products.AddAsync(entity, ct);

        var readDto = new ProductReadDto(
            created.Id, created.Name, created.Price, created.Stock,
            created.CategoryId, created.Category?.Name ?? string.Empty
        );

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, readDto);
    }
}