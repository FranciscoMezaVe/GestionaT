using GestionaT.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace GestionaT.Persistence.Common
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ICollection<Business> OwnedBusinesses { get; set; }
        public ICollection<Members> MemberBusinesses { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
        public UserImage Image { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
    }

}
