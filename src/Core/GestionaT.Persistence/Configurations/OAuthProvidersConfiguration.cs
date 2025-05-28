using GestionaT.Domain.Entities;
using GestionaT.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionaT.Persistence.Configurations
{
    public class OAuthProvidersConfiguration : IEntityTypeConfiguration<OAuthProviders>
    {
        public void Configure(EntityTypeBuilder<OAuthProviders> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne<ApplicationUser>()
                .WithOne(u => u.Provider)
                .HasForeignKey<OAuthProviders>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
