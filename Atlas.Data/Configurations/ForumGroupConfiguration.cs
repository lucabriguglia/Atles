using Atlas.Domain;
using Atlas.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Data.Configurations
{
    public class ForumGroupConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
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