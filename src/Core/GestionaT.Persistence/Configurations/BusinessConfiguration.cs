using GestionaT.Domain.Entities;
using GestionaT.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionaT.Persistence.Configurations
{
    public class BusinessConfiguration : IEntityTypeConfiguration<Business>
    {
        public void Configure(EntityTypeBuilder<Business> builder)
        {
            builder.HasKey(b => b.Id);

            builder.HasOne<ApplicationUser>()
                .WithMany(b => b.OwnedBusinesses)
                .HasForeignKey(b => b.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
