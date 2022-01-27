using Atles.Domain.Models.Posts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atles.Data.Configurations
{
    public class PostReactionCountConfiguration : IEntityTypeConfiguration<PostReaction>
    {
        public void Configure(EntityTypeBuilder<PostReaction> builder)
        {
            builder.ToTable("PostReaction");

            builder.HasKey(x => new { x.PostId, x.Type });

            builder
                .HasOne(x => x.Post)
                .WithMany(x => x.PostReactions)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }
    }
}