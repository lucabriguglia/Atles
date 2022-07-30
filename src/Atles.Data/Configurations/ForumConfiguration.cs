using Atles.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atles.Data.Configurations
{
    public class ForumConfiguration : IEntityTypeConfiguration<Forum>
    {
        public void Configure(EntityTypeBuilder<Forum> builder)
        {
            builder.ToTable("Forum");

            builder
                .HasOne(x => x.Category)
                .WithMany(x => x.Forums)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder
                .HasOne(x => x.PermissionSet)
                .WithMany(x => x.Forums)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}