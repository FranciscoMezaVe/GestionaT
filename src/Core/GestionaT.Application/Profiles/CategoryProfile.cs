using AutoMapper;
using GestionaT.Application.Features.Categories.Commands.CreateCategory;
using GestionaT.Domain.Entities;

namespace GestionaT.Application.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CreateCategoryCommand, Category>();
            CreateMap<Category, CreateCategoryCommand>();
        }
    }
}
