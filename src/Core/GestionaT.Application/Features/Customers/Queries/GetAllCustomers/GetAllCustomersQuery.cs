using FluentResults;
using MediatR;

namespace GestionaT.Application.Features.Customers.Queries.GetAllCustomers
{
    public class GetAllCustomersQuery : IRequest<Result<List<CustomerResponse>>>
    {
        public Guid BusinessId { get; set; }

        public GetAllCustomersQuery(Guid businessId)
        {
            BusinessId = businessId;
        }
    }
}