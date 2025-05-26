using FluentResults;
using GestionaT.Application.Common.Pagination;
using MediatR;

namespace GestionaT.Application.Features.Sales.Queries.GetAllSales
{
    public record GetAllSalesQuery(Guid BusinessId, SalesFilters Filters, PaginationFilters PaginationFilters) : IRequest<Result<PaginatedList<SalesResponse>>>;
}
