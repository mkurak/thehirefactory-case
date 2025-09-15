using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using TheHireFactory.ECommerce.Domain.Abstractions;
using TheHireFactory.ECommerce.Domain.Entities;
using TheHireFactory.ECommerce.Api.Dtos;

namespace TheHireFactory.ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class ProductController(IProductRepository products, IMapper mapper) : ControllerBase
{
    private readonly IProductRepository _products = products;
    private readonly IMapper _mapper = mapper;

    /// <summary>Ürünleri kategori bilgisiyle döner.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductReadDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var list = await _products.ListWithCategoryAsync(ct);
        var dto = _mapper.Map<IEnumerable<ProductReadDto>>(list);
        return Ok(dto);
    }

    /// <summary>Id ile tek ürün döner.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var entity = await _products.GetByIdAsync(id, ct);
        if (entity is null) return NotFound();
        return Ok(_mapper.Map<ProductReadDto>(entity));
    }

    /// <summary>Yeni ürün oluşturur.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] ProductCreateDto dto, CancellationToken ct)
    {
        // FluentValidation otomatik tetiklenecek (geçersiz ise 400 döner)
        var entity = _mapper.Map<Product>(dto);

        var created = await _products.AddAsync(entity, ct);
        // Eğer repo AddAsync() içinde SaveChanges yapmıyorsa, UoW/DbContext save burada çağrılır.
        // Bizim altyapıda SaveChanges repo tarafında ise bu satıra gerek yok.

        var read = _mapper.Map<ProductReadDto>(created);
        return CreatedAtAction(nameof(GetById), new { id = read.Id }, read);
    }
}