using Atlify.Domain.Posts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlify.Data.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Post");

            builder
                .HasOne(x => x.Forum)
                .WithMany(x => x.Posts)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder
                .HasOne(x => x.Member)
                .WithMany(x => x.Posts)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }
    }
}
