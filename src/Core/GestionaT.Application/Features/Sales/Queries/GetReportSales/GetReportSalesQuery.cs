using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Sales.Queries.GetReportSales
{
    public record GetReportSalesQuery(Guid BusinessId, SalesFilters Filters) : IRequest<Result<byte[]>>;
}
