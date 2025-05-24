using AutoMapper;
using GestionaT.Application.Features.Members;
using GestionaT.Application.Features.Members.Commands.CreateMembersCommand;
using GestionaT.Domain.Entities;

namespace GestionaT.Application.Profiles
{
    public class MembersProfile : Profile
    {
        public MembersProfile()
        {
            CreateMap<CreateMembersCommand, Members>();
            CreateMap<Members, MembersResponse>();

            CreateMap<Members, MembersResponse>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));
        }
    }
}
