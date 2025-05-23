using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Members.Commands.UpdateMemberRoleCommand
{
    public record UpdateMemberRoleCommand(Guid BusinessId, Guid MemberId, UpdateMemberRoleDto RoleDto) : IRequest<Result>;
}