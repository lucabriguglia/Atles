using Atles.Domain.Posts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atles.Data.Configurations
{
    public class PostReactionCountConfiguration : IEntityTypeConfiguration<PostReactionCount>
    {
        public void Configure(EntityTypeBuilder<PostReactionCount> builder)
        {
            builder.ToTable("PostReactionCount");

            builder.HasKey(x => new { x.PostId, x.Type });

            builder
                .HasOne(x => x.Post)
                .WithMany(x => x.PostReactionCounts)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }
    }
}