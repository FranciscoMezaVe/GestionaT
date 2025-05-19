using GestionaT.Domain.Abstractions;

namespace GestionaT.Domain.Entities
{
    public class Category : BaseEntity
    {
        public required string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
