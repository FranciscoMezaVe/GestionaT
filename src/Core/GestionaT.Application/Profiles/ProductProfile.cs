using AutoMapper;
using GestionaT.Application.Features.Products.Commands.CreateProduct;
using GestionaT.Application.Features.Products.Commands.UpdateProduct;
using GestionaT.Application.Features.Products.Queries;
using GestionaT.Domain.Entities;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<CreateProductCommandRequest, Product>();

        CreateMap<UpdateProductCommandRequest, Product>()
            .ForMember(dest => dest.CategoryId, opt => opt.Ignore());

        CreateMap<Product, ProductResponse>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
    }
}
