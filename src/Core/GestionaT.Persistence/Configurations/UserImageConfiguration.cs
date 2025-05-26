using GestionaT.Domain.Entities;
using GestionaT.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionaT.Persistence.Configurations
{
    public class UserImageConfiguration : IEntityTypeConfiguration<UserImage>
    {
        public void Configure(EntityTypeBuilder<UserImage> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne<ApplicationUser>()
                .WithOne(u => u.Image)
                .HasForeignKey<UserImage>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade); // o Restrict, según tu caso
        }
    }
}
