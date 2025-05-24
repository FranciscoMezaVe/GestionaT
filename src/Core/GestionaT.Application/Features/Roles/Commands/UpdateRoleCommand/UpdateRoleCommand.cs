using FluentResults;
using GestionaT.Domain.Entities;
using MediatR;

namespace GestionaT.Application.Features.Roles.Commands.UpdateRoleCommand
{
    public record UpdateRoleCommand(UpdateRoleCommandRequest Request, Guid Id, Guid BusinessId) : IRequest<Result>;
}