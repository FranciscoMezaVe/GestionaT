using GestionaT.Domain.Entities;
using GestionaT.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionaT.Persistence.Configurations
{
    public class MemberConfiguration : IEntityTypeConfiguration<Members>
    {
        public void Configure(EntityTypeBuilder<Members> builder)
        {
            builder.HasKey(b => b.Id);

            builder.HasIndex(m => new { m.UserId, m.BusinessId }).IsUnique();

            builder
                .HasOne<ApplicationUser>()
                .WithMany(b => b.MemberBusinesses)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne<Business>()
                .WithMany(b => b.Members)
                .HasForeignKey(m => m.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(m => m.Role)
                .WithMany()
                .HasForeignKey(m => m.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
