using AutoMapper;
using TheHireFactory.ECommerce.Api.Dtos;
using TheHireFactory.ECommerce.Domain.Entities;

namespace TheHireFactory.ECommerce.Api.Mappings;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ProductCreateDto, Product>();
        CreateMap<Product, ProductReadDto>();
    }
}