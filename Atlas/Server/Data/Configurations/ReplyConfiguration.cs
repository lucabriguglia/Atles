using Atlas.Server.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Data.Configurations
{
    public class ReplyConfiguration : IEntityTypeConfiguration<Reply>
    {
        public void Configure(EntityTypeBuilder<Reply> builder)
        {
            builder.ToTable("Reply");

            builder
                .HasOne(x => x.Topic)
                .WithMany(x => x.Replies)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder
                .HasOne(x => x.Member)
                .WithMany(x => x.Replies)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }
    }
}