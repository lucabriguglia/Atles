using Atles.Domain.PostLikes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atles.Data.Configurations
{
    public class PostLikeConfiguration : IEntityTypeConfiguration<PostLike>
    {
        public void Configure(EntityTypeBuilder<PostLike> builder)
        {
            builder.ToTable("PostLike");

            builder.HasKey(x => new { x.PostId, x.UserId });

            builder
                .HasOne(x => x.Post)
                .WithMany(x => x.PostLikes)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder
                .HasOne(x => x.User)
                .WithMany(x => x.PostLikes)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }
    }
}