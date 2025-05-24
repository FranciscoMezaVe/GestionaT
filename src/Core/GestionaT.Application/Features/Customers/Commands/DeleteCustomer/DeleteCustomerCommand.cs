using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Customers.Commands.DeleteCustomer
{
    public record DeleteCustomerCommand(Guid CustomerId, Guid BusinessId) : IRequest<Result>;
}
