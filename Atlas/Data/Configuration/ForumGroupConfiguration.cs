using Atlas.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Data.Configuration
{
    public class ForumGroupConfiguration : IEntityTypeConfiguration<ForumGroup>
    {
        public void Configure(EntityTypeBuilder<ForumGroup> builder)
        {
            builder.ToTable("ForumGroup");

            builder
                .HasOne(x => x.Site)
                .WithMany(x => x.ForumGroups)
                .IsRequired();
        }
    }
}