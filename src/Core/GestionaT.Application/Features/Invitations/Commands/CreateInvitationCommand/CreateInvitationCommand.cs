using FluentResults;
using GestionaT.Application.Features.Invitations.Commands.CreateInvitationCommand;
using MediatR;

namespace GestionaT.Application.Features.Invitations.Commands.CreateInvitation
{
    public record CreateInvitationCommand(Guid BusinessId, CreateInvitationCommandDto Request) : IRequest<Result<Guid>>;
}