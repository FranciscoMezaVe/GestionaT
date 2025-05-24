using MediatR;
using FluentResults;

namespace GestionaT.Application.Features.Customers.Commands.CreateCustomer
{
    public record CreateCustomerCommand(CreateCustomerCommandRequest Request, Guid BusinessId) : IRequest<Result<Guid>>;
}