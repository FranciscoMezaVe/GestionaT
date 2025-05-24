using FluentResults;
using GestionaT.Application.Features.Invitations.Queries.GetAllInvitations;
using MediatR;

namespace GestionaT.Application.Features.Invitations.Queries.GetAllInvitations
{
    public record GetAllInvitationsQuery(Guid businessId) : IRequest<Result<IEnumerable<InvitationResponse>>>;
}
