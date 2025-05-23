using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Roles.Queries.GetRoleByIdQuery
{
    public record GetRoleByIdQuery(Guid BusinessId, Guid RoleId) : IRequest<Result<RolesResponse>>;
}
