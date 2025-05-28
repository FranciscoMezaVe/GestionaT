using GestionaT.Domain.Abstractions;

namespace GestionaT.Domain.Entities
{
    public class OAuthProviders : BaseEntity
    {
        public Guid UserId { get; set; }
        public string? ExternalProvider { get; set; } // "Facebook", "Google"
        public string? ExternalProviderId { get; set; }
        public bool IsExternal => ExternalProvider != null;
        public bool isRevone { get; set; } = false;
    }
}
