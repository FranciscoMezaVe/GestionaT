using GestionaT.Domain.Abstractions;

namespace GestionaT.Domain.Entities
{
    public class Sale : BaseEntity
    {
        public DateTime Date { get; set; }

        public Guid BusinessId { get; set; }
        public Business Business { get; set; }

        public Guid CustomerId { get; set; }
        public required Customer Customer { get; set; }

        public ICollection<SaleProduct> SaleProducts { get; set; }
    }
}
