using FluentResults;
using GestionaT.Application.Common.Pagination;
using MediatR;

namespace GestionaT.Application.Features.Customers.Queries.GetAllCustomers
{
    public class GetAllCustomersQuery : IRequest<Result<PaginatedList<CustomerResponse>>>
    {
        public Guid BusinessId { get; set; }
        public PaginationFilters Filters { get; set; }

        public GetAllCustomersQuery(Guid businessId, PaginationFilters filters)
        {
            BusinessId = businessId;
            Filters = filters;
        }
    }
}