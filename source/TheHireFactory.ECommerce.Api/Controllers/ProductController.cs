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
        if (p is null) return NotFound();

        var dto = new ProductReadDto(
            p.Id, p.Name, p.Price, p.Stock, p.CategoryId, p.Category!.Name
        );

        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ProductCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return ValidationProblem("Name zorunludur.");
        if (dto.Price < 0)
            return ValidationProblem("Price 0'dan küçük olamaz.");
        if (dto.Stock < 0)
            return ValidationProblem("Stock 0'dan küçük olamaz.");

        var entity = new Product
        {
            Name = dto.Name.Trim(),
            Price = dto.Price,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId
        };

        var created = await products.AddAsync(entity);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            new ProductReadDto(created.Id, created.Name, created.Price, created.Stock, created.CategoryId, created.Category?.Name ?? "")
        );
    }
}