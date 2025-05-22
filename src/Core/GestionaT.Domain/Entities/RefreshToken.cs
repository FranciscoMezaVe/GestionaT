using GestionaT.Domain.Abstractions;

namespace GestionaT.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public required string Token { get; set; }
        public required Guid UserId { get; set; }
        public required DateTime Expires { get; set; }
        public bool IsRevoked { get; set; } = false;
        public bool IsUsed { get; set; } = false;

        public bool IsValid()
        {
            return !IsUsed && !IsRevoked && Expires > DateTime.UtcNow;
        }
    }
}
