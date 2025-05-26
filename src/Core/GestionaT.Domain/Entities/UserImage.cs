using GestionaT.Domain.Abstractions;

namespace GestionaT.Domain.Entities
{
    public class UserImage : BaseEntity
    {
        public Guid UserId { get; set; }
        public required string ImageUrl { get; set; }
        public required string PublicId { get; set; } // Útil para eliminar desde Cloudinary
    }
}
