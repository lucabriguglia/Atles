using Atles.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atles.Data.Configurations
{
    public class PostReactionSummaryConfiguration : IEntityTypeConfiguration<PostReactionSummary>
    {
        public void Configure(EntityTypeBuilder<PostReactionSummary> builder)
        {
            builder.ToTable("PostReactionSummary");

            builder.HasKey(x => new { x.PostId, x.Type });

            builder
                .HasOne(x => x.Post)
                .WithMany(x => x.PostReactionSummaries)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }
    }
}