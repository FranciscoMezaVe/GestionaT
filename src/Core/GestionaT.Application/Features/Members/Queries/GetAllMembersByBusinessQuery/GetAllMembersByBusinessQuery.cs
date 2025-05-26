using FluentResults;
using GestionaT.Application.Common.Pagination;
using MediatR;

namespace GestionaT.Application.Features.Members.Queries.GetAllMembersByBusiness
{
    public record GetAllMembersByBusinessQuery(Guid BusinessId, PaginationFilters PaginationFilters) : IRequest<Result<PaginatedList<MembersResponse>>>;
}