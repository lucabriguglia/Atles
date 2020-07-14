using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Data.Configurations
{
    public class ForumGroupConfiguration : IEntityTypeConfiguration<ForumGroup>
    {
        public void Configure(EntityTypeBuilder<ForumGroup> builder)
        {
            builder.ToTable("ForumGroup");

            builder
                .HasOne(x => x.Site)
                .WithMany(x => x.ForumGroups)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder
                .HasOne(x => x.PermissionSet)
                .WithMany(x => x.ForumGroups)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}