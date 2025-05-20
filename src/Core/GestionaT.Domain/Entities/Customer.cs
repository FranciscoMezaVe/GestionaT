using GestionaT.Domain.Abstractions;

namespace GestionaT.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public required string Name { get; set; }
        public Guid BusinessId { get; set; }
        public required Business Business { get; set; }
        public ICollection<Sale> Sales { get; set; }
    }
}
