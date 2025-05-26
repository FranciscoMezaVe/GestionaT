using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Sales.Queries.GetSaleById
{
    public record GetSaleByIdQuery(Guid businessId, Guid SaleId) : IRequest<Result<SalesResponse>>;
}
