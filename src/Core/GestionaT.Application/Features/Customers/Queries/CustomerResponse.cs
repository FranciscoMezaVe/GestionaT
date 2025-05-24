using GestionaT.Domain.Abstractions;

namespace GestionaT.Application.Features.Customers
{
    public class CustomerResponse : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Rfc { get; set; }
        public string? ZipCode { get; set; }
        public string? Address { get; set; }
        public string BusinessName { get; set; } = null!;
    }
}