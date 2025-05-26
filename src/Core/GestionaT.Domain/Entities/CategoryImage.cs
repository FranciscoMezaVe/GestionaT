using GestionaT.Domain.Abstractions;

namespace GestionaT.Domain.Entities
{
    public class CategoryImage : BaseEntity
    {
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }
        public required string ImageUrl { get; set; }
        public required string PublicId { get; set; } // Útil para eliminar desde Cloudinary
    }
}
