using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Customers.Queries.GetCustomerById
{
    public class GetCustomerByIdQuery : IRequest<Result<CustomerResponse>>
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }

        public GetCustomerByIdQuery(Guid id, Guid businessId)
        {
            Id = id;
            BusinessId = businessId;
        }
    }
}
