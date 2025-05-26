using GestionaT.Domain.Abstractions;

namespace GestionaT.Application.Features.Sales.Queries
{
    public class SalesResponse : BaseEntity
    {
        public DateTime Date { get; set; }
        public Guid CustomerId { get; set; }
        public required string NameCustomer { get; set; }
    }
}
