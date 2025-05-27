using AutoMapper;
using GestionaT.Application.Features.Business.Commands.CreateBusinessCommand;
using GestionaT.Application.Features.Business.Commands.UpdateBusinessCommand;
using GestionaT.Application.Features.Business.Queries;
using GestionaT.Domain.Entities;

namespace GestionaT.Application.Profiles
{
    public class BusinessProfile : Profile
    {
        public BusinessProfile()
        {
            CreateMap<CreateBusinessCommand, Business>();

            CreateMap<Business, BusinessReponse>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Image.ImageUrl));

            CreateMap<UpdateBusinessDto, Business>();
        }
    }
}
