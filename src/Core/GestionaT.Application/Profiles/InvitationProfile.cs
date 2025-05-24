using AutoMapper;
using GestionaT.Application.Features.Invitations.Commands.CreateInvitationCommand;
using GestionaT.Application.Features.Invitations.Queries.GetAllInvitations;
using GestionaT.Domain.Entities;

namespace GestionaT.Application.Profiles
{
    public class InvitationProfile : Profile
    {
        public InvitationProfile()
        {
            CreateMap<CreateInvitationCommandDto, Invitation>();

            CreateMap<Invitation, InvitationResponse>()
                .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => src.Business.Name));
        }
    }
}