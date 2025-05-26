using FluentResults;
using GestionaT.Application.Common.Pagination;
using MediatR;

namespace GestionaT.Application.Features.Business.Queries.GetAllBusinessesQuery
{
    public record GetAllBusinessesQuery(Guid UserId, PaginationFilters Filters) : IRequest<Result<PaginatedList<BusinessReponse>>>;
}
