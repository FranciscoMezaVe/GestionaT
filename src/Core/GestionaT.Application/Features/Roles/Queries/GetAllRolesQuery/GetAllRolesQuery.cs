using FluentResults;
using GestionaT.Application.Common.Pagination;
using MediatR;

namespace GestionaT.Application.Features.Roles.Queries.GetAllRolesQuery
{
    public record GetAllRolesQuery(Guid businessId, PaginationFilters PaginationFilters) : IRequest<Result<PaginatedList<RolesResponse>>>;
}
