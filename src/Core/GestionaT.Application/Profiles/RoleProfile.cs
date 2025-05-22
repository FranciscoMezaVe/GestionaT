using AutoMapper;
using GestionaT.Application.Features.Roles;
using GestionaT.Application.Features.Roles.Commands.CreateRolesCommand;
using GestionaT.Domain.Entities;

namespace GestionaT.Application.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<CreateRolesCommand, Role>();
            CreateMap<Role, RolesResponse>();
        }
    }
}
