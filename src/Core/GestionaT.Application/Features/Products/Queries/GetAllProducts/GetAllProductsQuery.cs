using FluentResults;
using GestionaT.Application.Common.Pagination;
using GestionaT.Application.Features.Products.Queries;
using MediatR;

namespace GestionaT.Application.Features.Products.Queries.GetAllProducts
{
    public record GetAllProductsQuery(Guid BusinessId, PaginationFilters PaginationFilters) : IRequest<Result<PaginatedList<ProductResponse>>>;
}
