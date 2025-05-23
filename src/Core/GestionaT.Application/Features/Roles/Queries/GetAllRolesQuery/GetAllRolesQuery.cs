using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Roles.Queries.GetAllRolesQuery
{
    public record GetAllRolesQuery(Guid businessId) : IRequest<Result<IEnumerable<RolesResponse>>>;
}
