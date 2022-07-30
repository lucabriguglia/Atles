using Atles.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atles.Data.Configurations;

public class PostReactionConfiguration : IEntityTypeConfiguration<PostReaction>
{
    public void Configure(EntityTypeBuilder<PostReaction> builder)
    {
        builder.ToTable("PostReaction");

        builder.HasKey(x => new { x.PostId, x.UserId });

        builder
            .HasOne(x => x.Post)
            .WithMany(x => x.PostReactions)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.PostReactions)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}