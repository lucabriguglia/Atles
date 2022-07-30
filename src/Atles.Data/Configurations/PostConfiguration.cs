using Atles.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atles.Data.Configurations
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
                .HasOne(x => x.CreatedByUser)
                .WithMany(x => x.Posts)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }
    }
}
