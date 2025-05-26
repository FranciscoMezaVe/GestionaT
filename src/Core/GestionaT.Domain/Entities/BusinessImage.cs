using GestionaT.Domain.Abstractions;

namespace GestionaT.Domain.Entities
{
    public class BusinessImage : BaseEntity
    {
        public Guid BusinessId { get; set; }
        public Business? Business { get; set; }
        public required string ImageUrl { get; set; }
        public required string PublicId { get; set; } // Útil para eliminar desde Cloudinary
    }
}
