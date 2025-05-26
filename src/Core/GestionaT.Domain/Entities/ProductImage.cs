using GestionaT.Domain.Abstractions;

namespace GestionaT.Domain.Entities
{
    public class ProductImage : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public required string ImageUrl { get; set; }
        public required string PublicId { get; set; } // Útil para eliminar desde Cloudinary
    }
}
