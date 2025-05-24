using FluentResults;
using GestionaT.Application.Features.Invitations.Queries.GetAllInvitations;
using GestionaT.Domain.Enums;
using MediatR;

namespace GestionaT.Application.Features.Invitations.Queries.GetAllInivitationsByUser
{
    public record GetAllInvitationByUserQuery(GetAllInvitationByUserQueryFilters Filters) :IRequest<Result<IEnumerable<InvitationResponse>>>;
}
