using AutoMapper;
using GestionaT.Application.Features.Products.Commands.CreateProduct;
using GestionaT.Application.Features.Products.Commands.UpdateProduct;
using GestionaT.Application.Features.Products.Queries;
using GestionaT.Domain.Entities;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<CreateProductCommandRequest, Product>()
            .ForMember(dest => dest.Images, opt => opt.Ignore());

        CreateMap<UpdateProductCommandRequest, Product>()
            .ForMember(dest => dest.CategoryId, opt => opt.Ignore());

        CreateMap<Product, ProductResponse>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                src.Images.Select(i => new ProductImages(i.ImageUrl, i.Id, i.PublicId)).ToList()));
    }
}
