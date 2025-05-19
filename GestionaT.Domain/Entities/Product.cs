using GestionaT.Domain.Abstractions;

namespace GestionaT.Domain.Entities
{
    public class Product : BaseEntity
    {
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public Guid BusinessId { get; set; }
        public required Business Business { get; set; }
        public Guid CategoryId { get; set; }
        public required Category Category { get; set; }
        public ICollection<SaleProduct> SaleProducts { get; set; }
    }
}
