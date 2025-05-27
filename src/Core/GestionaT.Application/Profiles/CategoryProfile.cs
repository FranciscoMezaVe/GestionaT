using AutoMapper;
using GestionaT.Application.Features.Categories.Commands.CreateCategory;
using GestionaT.Application.Features.Categories.Commands.UpdateCategory;
using GestionaT.Application.Features.Categories.Queries;
using GestionaT.Domain.Entities;

namespace GestionaT.Application.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CreateCategoryCommandRequest, Category>();

            CreateMap<Category, CategoryResponse>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Image.ImageUrl));

            CreateMap<UpdateCategoryCommandRequest, Category>();
        }
    }
}
