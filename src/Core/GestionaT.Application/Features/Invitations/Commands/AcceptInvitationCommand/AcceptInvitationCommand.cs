using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Invitations.Commands.AcceptInvitationCommand
{
    public record AcceptInvitationCommand(Guid InvitationId) : IRequest<Result>;
}
