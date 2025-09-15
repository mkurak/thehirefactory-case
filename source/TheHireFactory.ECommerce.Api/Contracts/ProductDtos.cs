namespace TheHireFactory.ECommerce.Api.Contracts;

public sealed record ProductCreateDto(
    string Name,
    decimal Price,
    int Stock,
    int CategoryId
);

public sealed record ProductReadDto(
    int Id,
    string Name,
    decimal Price,
    int Stock,
    int CategoryId,
    string CategoryName
);