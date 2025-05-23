using AutoMapper;
using GestionaT.Application.Features.Roles;
using GestionaT.Application.Features.Roles.Commands.CreateRolesCommand;
using GestionaT.Application.Features.Roles.Commands.UpdateRoleCommand;
using GestionaT.Domain.Entities;

namespace GestionaT.Application.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<CreateRolesCommand, Role>();
            CreateMap<Role, RolesResponse>();

            CreateMap<UpdateRoleCommand, Role>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // evitamos que AutoMapper cambie el ID por accidente
                .ForMember(dest => dest.Permissions, opt => opt.Ignore()) // también ignoramos navegación
                .ForMember(dest => dest.Business, opt => opt.Ignore());
        }
    }
}
