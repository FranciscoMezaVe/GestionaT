using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Roles.Commands.DeleteRoleCommand
{
    public record DeleteRoleCommand(Guid Id, Guid BusinessId) : IRequest<Result>;
}