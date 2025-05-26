using GestionaT.Domain.Abstractions;

namespace GestionaT.Domain.Entities
{
    public class Category : BaseEntity, ISoftDeletable
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public Guid BusinessId { get; set; }
        public Business Business { get; set; } = default!;

        public ICollection<Product> Products { get; set; } = new List<Product>();
        public Category? Image { get; set; }
        public bool IsDeleted { get; set; }
    }
}
