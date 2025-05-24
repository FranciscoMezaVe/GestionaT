using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Invitations.Commands.RejectInvitationCommand
{
    public record RejectInvitationCommand(Guid InvitationId) : IRequest<Result>;
}
