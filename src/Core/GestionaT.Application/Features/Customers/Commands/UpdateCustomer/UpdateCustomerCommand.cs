using MediatR;
using FluentResults;

namespace GestionaT.Application.Features.Customers.Commands.UpdateCustomer
{
    public record UpdateCustomerCommand(UpdateCustomerCommandRequest Request, Guid Id, Guid BusinessId) : IRequest<Result>;
}
