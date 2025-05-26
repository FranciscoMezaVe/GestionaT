using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Sales.Commands.CreateSales
{
    public record CreateSalesCommand(CreateSalesCommandRequest Request, Guid BusinessId) : IRequest<Result<Guid>>;
}
