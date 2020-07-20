using Atlas.Server.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Data.Configurations
{
    public class TopicConfiguration : IEntityTypeConfiguration<Topic>
    {
        public void Configure(EntityTypeBuilder<Topic> builder)
        {
            builder.ToTable("Topic");

            builder
                .HasOne(x => x.Forum)
                .WithMany(x => x.Topics)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder
                .HasOne(x => x.Member)
                .WithMany(x => x.Topics)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }
    }
}
