using FluentResults;
using GestionaT.Application.Common.Pagination;
using MediatR;

namespace GestionaT.Application.Features.Categories.Queries.GetAllCategories
{
    public record GetAllCategoriesQuery(Guid BusinessId, PaginationFilters Filters) : IRequest<Result<PaginatedList<CategoryResponse>>>;
}